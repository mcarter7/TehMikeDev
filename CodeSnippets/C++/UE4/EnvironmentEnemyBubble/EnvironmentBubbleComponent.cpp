// Copyright 2019 Desert Beagle

#include "EnvironmentBubbleComponent.h"
#include "Engine.h"
#include "Engine/World.h"
#include "GameFramework/Actor.h"
#include "TimeDilationComponent.h"

// Sets default values for this component's properties
UEnvironmentBubbleComponent::UEnvironmentBubbleComponent()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;

	_collision = CreateDefaultSubobject<USphereComponent>(TEXT("SphereCollision"));
	_collision->SetCollisionResponseToAllChannels(ECollisionResponse::ECR_Overlap);
	_collision->SetSphereRadius(ActiveRadius);
	_collision->OnComponentBeginOverlap.AddDynamic(this, &UEnvironmentBubbleComponent::OnSphereOverlap);
	_collision->OnComponentEndOverlap.AddDynamic(this, &UEnvironmentBubbleComponent::OnSphereEndOverlap);
}


// Called when the game starts
void UEnvironmentBubbleComponent::BeginPlay()
{
	Super::BeginPlay();

	WRLD = GetWorld();
	TheOwner = GetOwner();
	ThePlayer = WRLD->GetFirstPlayerController()->GetPawn();

	if (TheOwner)
	{
		_collision->SetWorldLocation(TheOwner->GetActorLocation());
	}

	if (bubbleCurve)
	{
		FOnTimelineFloat TimelineCallback;
		FOnTimelineEventStatic TimelineFinishedCallback;

		TimelineCallback.BindUFunction(this, FName("ControlBubbleDecay"));
		TimelineFinishedCallback.BindUFunction(this, FName("DestoryBubble"));

		bubbleTimeline.AddInterpFloat(bubbleCurve, TimelineCallback);
		bubbleTimeline.SetTimelineFinishedFunc(TimelineFinishedCallback);
	}

	CurrentDelay = Delay;
	rootOffset = 0.0f;

	ensure(ComponentHasTag("BoxyFoxy"));
}


void UEnvironmentBubbleComponent::InitializeComponent()
{
	ComponentTags.Add(FName("BoxyFoxy"));
}

// Called every frame
void UEnvironmentBubbleComponent::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);

	bubbleTimeline.TickTimeline(DeltaTime);

	//Current Delay until bubble can activate again
	if (CurrentDelay < Delay)
		CurrentDelay += DeltaTime;

	if (TheOwner)
	{
		_collision->SetWorldLocation(TheOwner->GetActorLocation());
	}

	if (disablePlayerBubble) 
	{
		UTimeDilationComponent* timeDilation = Cast<UTimeDilationComponent>(ThePlayer->GetComponentByClass(UTimeDilationComponent::StaticClass()));
		if (timeDilation != nullptr)
		{
			timeDilation->DestoryBubble();
		}
	}
}

// Control bubble's decay from timeline
void UEnvironmentBubbleComponent::ControlBubbleDecay()
{
	if (IsActivated && spawnedBubble != nullptr)
	{
		//Get and set timeline value
		timelineValue = bubbleTimeline.GetPlaybackPosition();
		curveFloatValue = bubbleCurve->GetFloatValue(timelineValue);

		//Set the bubble location
		auto bubbleLoc = TheOwner->GetActorLocation();
		BubbleLocation = FVector(bubbleLoc.X, bubbleLoc.Y, bubbleLoc.Z);
		//BubbleRotation = TheOwner->GetActorRotation();
		spawnedBubble->SetActorLocation(BubbleLocation);
		//spawnedBubble->SetActorRotation(BubbleRotation);

		//Shrink the bubble scale
		//FVector bubbleScale = FVector(1 - curveFloatValue, 1 - curveFloatValue, 1 - curveFloatValue);
		//spawnedBubble->SetActorScale3D(bubbleScale);
	}
}

// Destroy current bubble
void UEnvironmentBubbleComponent::DestoryBubble()
{
	if (spawnedBubble != nullptr)
	{
		spawnedBubble->Destroy();
		spawnedBubble = nullptr;
		IsActivated = false;
		ThePlayer->CustomTimeDilation = 1.00f;
	}
}

void UEnvironmentBubbleComponent::OnSphereOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult & SweepResult)
{
	if (ThePlayer != nullptr) 
	{
		bool compHasTag = OtherComp->ComponentHasTag("TimeRewind");
		bool selfCheck = OtherComp->ComponentHasTag("BoxyFoxy");

		if (!compHasTag && !selfCheck && !OtherActor->GetName().Contains("TriggerBox"))
		{
			if (OtherActor->GetName() == ThePlayer->GetName()) 
			{
				OtherActor->CustomTimeDilation = 0.05f;
				
				UTimeDilationComponent* timeDilation = Cast<UTimeDilationComponent>(ThePlayer->GetComponentByClass(UTimeDilationComponent::StaticClass()));				
				if (timeDilation != nullptr) 
				{
					timeDilation->DestoryBubble();
					disablePlayerBubble = true;
				}
			}

			if (ActorToSpawn != nullptr && !IsActivated && (CurrentDelay >= Delay))
			{
				IsActivated = true;
				auto bubbleLoc = TheOwner->GetActorLocation();
				BubbleLocation = FVector(bubbleLoc.X, bubbleLoc.Y, bubbleLoc.Z);
				//BubbleRotation = TheOwner->GetActorRotation();
				spawnedBubble = WRLD->SpawnActor(ActorToSpawn, &BubbleLocation);

				//Start the timeline curve
				bubbleTimeline.PlayFromStart();

				//Start delay
				CurrentDelay = 0;
			}
		}
	}
}

void UEnvironmentBubbleComponent::OnSphereEndOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex)
{
	if (ThePlayer != nullptr)
	{
		bool compHasTag = OtherComp->ComponentHasTag("TimeRewind");
		bool selfCheck = OtherComp->ComponentHasTag("BoxyFoxy");

		if (!compHasTag && !selfCheck)
		{
			if (OtherActor->GetName() == ThePlayer->GetName()) 
			{
				disablePlayerBubble = false;
				OtherActor->CustomTimeDilation = 1.00f;
			}
		}
	}
}