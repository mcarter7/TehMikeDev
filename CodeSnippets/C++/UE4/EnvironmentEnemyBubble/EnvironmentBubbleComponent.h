// Copyright 2019 Desert Beagle

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "Components/TimelineComponent.h"
#include "Components/SphereComponent.h"
#include "EnvironmentBubbleComponent.generated.h"


UCLASS(ClassGroup = (Custom), meta = (BlueprintSpawnableComponent))
class DBGAME_API UEnvironmentBubbleComponent : public UActorComponent
{
	GENERATED_BODY()

public:
	// Sets default values for this component's properties
	UEnvironmentBubbleComponent();

private:
	UPROPERTY(VisibleDefaultsOnly, Category = "Projectile")
	class USphereComponent* _collision;

protected:
	// Called when the game starts
	virtual void BeginPlay() override;

	UWorld *WRLD;

	const AActor *TheOwner;

	APawn *ThePlayer;

	AActor* spawnedBubble;

	FVector BubbleLocation;

	FRotator BubbleRotation;

	float bubbleTimer;

	float timelineValue;

	float curveFloatValue;

	float rootOffset;

	FTimeline bubbleTimeline;

	float CurrentDelay;

	bool disablePlayerBubble = false;

public:
	virtual void InitializeComponent() override;

	// Called every frame
	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;

	UFUNCTION()
	void OnSphereOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult & SweepResult);

	UFUNCTION()
	void OnSphereEndOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex);

	// Control bubble decay through timeline
	UFUNCTION(BlueprintCallable)
	void ControlBubbleDecay();

	// Destroy current bubble
	UFUNCTION(BlueprintCallable)
	void DestoryBubble();

	//Is the bubble activated
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	bool IsActivated;

	UPROPERTY(EditAnywhere)
	UCurveFloat* bubbleCurve;

	UPROPERTY(EditAnywhere)
	float ActiveRadius = 230.0f;

	//The bubble actor to spawn
	UPROPERTY(EditAnywhere)
	TSubclassOf<AActor> ActorToSpawn;

	//Delay before bubble can be activated again
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	float Delay = 0;
};
