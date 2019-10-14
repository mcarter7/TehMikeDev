// Fill out your copyright notice in the Description page of Project Settings.

#include "TimeDilationComponent.h"
#include "Runtime/Engine/Classes/GameFramework/PlayerController.h"
#include "Engine/World.h"
#include "DBGameCharacter.h"

// Sets default values for this component's properties
UTimeDilationComponent::UTimeDilationComponent()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;

	IsActivated = false;
}


// Called when the game starts
void UTimeDilationComponent::BeginPlay()
{
	Super::BeginPlay();

	WRLD = GetWorld();
	if (WRLD == nullptr || !WRLD->IsValidLowLevelFast())
	{
		return;
	}

	//Binding for Time Dilation Bubble
	if (!WRLD->IsPlayingReplay()) 
	{
		APlayerController* playerController = WRLD->GetFirstPlayerController();
		if (playerController != nullptr)
		{
			playerController->InputComponent->BindAction("TimeBubble", IE_Pressed, this, &UTimeDilationComponent::ActivateTimeDilationBubble);
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

		mPlayer = Cast<ADBGameCharacter>(GetOwner());
		check(mPlayer != nullptr);

		CurrentDelay = Delay;
	}
	
}


// Called every frame
void UTimeDilationComponent::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);

	bubbleTimeline.TickTimeline(DeltaTime);

	//Current Delay until bubble can activate again
	if (CurrentDelay < Delay)
		CurrentDelay += DeltaTime;

}

// Activate Time Dilation Bubble
void UTimeDilationComponent::ActivateTimeDilationBubble()
{
	if (mPlayer->AbilityLock[EAbilityType::TimeBubble] && !IsActivated && (CurrentDelay >= Delay))
	{
		BubbleLocation = WRLD->GetFirstPlayerController()->GetPawn()->GetActorLocation();
		BubbleRotation = WRLD->GetFirstPlayerController()->GetPawn()->GetActorRotation();

		if (ActorToSpawn != nullptr) 
		{
			IsActivated = true;
			spawnedBubble = WRLD->SpawnActor(ActorToSpawn, &BubbleLocation, &BubbleRotation);

			//Start the timeline curve
			bubbleTimeline.PlayFromStart();

			CurrentDelay = 0;
			playerBubbleAnimation = true;
		}
	}
}

// Control bubble's decay from timeline
void UTimeDilationComponent::ControlBubbleDecay()
{
	if (IsActivated && spawnedBubble != nullptr)
	{
		//Get and set timeline value
		timelineValue = bubbleTimeline.GetPlaybackPosition();
		curveFloatValue = bubbleCurve->GetFloatValue(timelineValue);

		//Set the bubble location
		BubbleLocation = WRLD->GetFirstPlayerController()->GetPawn()->GetActorLocation();

		if(spawnedBubble != nullptr)
			spawnedBubble->SetActorLocation(BubbleLocation);

		//Shrink the bubble scale
		FVector bubbleScale = FVector(1 - curveFloatValue, 1 - curveFloatValue, 1 - curveFloatValue);

		if(spawnedBubble != nullptr)
			spawnedBubble->SetActorScale3D(bubbleScale);
	}
}

// Destroy current bubble
void UTimeDilationComponent::DestoryBubble()
{
	if (spawnedBubble != nullptr)
	{
		spawnedBubble->Destroy();
		spawnedBubble = nullptr;
		IsActivated = false;
		playerBubbleAnimation = false;
	}
}

