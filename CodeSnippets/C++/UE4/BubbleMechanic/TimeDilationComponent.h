// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "Components/TimelineComponent.h"
#include "TimeDilationComponent.generated.h"



UCLASS( ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class DBGAME_API UTimeDilationComponent : public UActorComponent
{
	GENERATED_BODY()

public:	
	// Sets default values for this component's properties
	UTimeDilationComponent();

protected:
	// Called when the game starts
	virtual void BeginPlay() override;

	AActor *spawnedBubble;

	UWorld *WRLD;

	FVector BubbleLocation;

	FRotator BubbleRotation;

	float bubbleTimer;
	float timelineValue;
	float curveFloatValue;
	FTimeline bubbleTimeline;
	float CurrentDelay;
	float bubbleMaxTimeCount;

	class ADBGameCharacter* mPlayer = nullptr;

public:	
	// Called every frame
	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;

	// Activate Time Dilation Bubble
	UFUNCTION(BlueprintCallable)
	void ActivateTimeDilationBubble();

	// Control bubble decay through timeline
	UFUNCTION(BlueprintCallable)
	void ControlBubbleDecay();

	// Destroy current bubble
	UFUNCTION(BlueprintCallable)
	void DestoryBubble();
	
	//Is the bubble activated
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	bool IsActivated;

	//The bubble actor to spawn
	UPROPERTY(EditAnywhere)
	TSubclassOf<AActor> ActorToSpawn;

	UPROPERTY(EditAnywhere)
	UCurveFloat* bubbleCurve;

	//Delay before bubble can be activated again
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	float Delay = 0;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	bool playerBubbleAnimation = false;
};

