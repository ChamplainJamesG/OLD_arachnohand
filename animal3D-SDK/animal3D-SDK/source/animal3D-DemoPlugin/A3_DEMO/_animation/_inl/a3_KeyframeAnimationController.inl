/*
	Copyright 2011-2019 Daniel S. Buckstein

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at

		http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/

/*
	animal3D SDK: Minimal 3D Animation Framework
	By Daniel S. Buckstein
	
	a3_KeyframeAnimationController.h
	inline definitions for keyframe animation controller.
*/

#ifdef __ANIMAL3D_KEYFRAMEANIMATIONCONTROLLER_H
#ifndef __ANIMAL3D_KEYFRAMEANIMATIONCONTROLLER_INL
#define __ANIMAL3D_KEYFRAMEANIMATIONCONTROLLER_INL


//-----------------------------------------------------------------------------

// utility to set next keyframe index
inline a3i32 a3clipController_internalSetKeyframe(a3_ClipController* clipCtrl, const a3ui32 keyframeIndex_clip)
{
	// ****TO-DO: uncomment
	clipCtrl->keyframeIndex_clip = keyframeIndex_clip;
	clipCtrl->keyframePtr = clipCtrl->clipPtr->keyframeListBasePtr_pool + clipCtrl->keyframeIndex_clip;
	return keyframeIndex_clip;
}


//-----------------------------------------------------------------------------
#define yoink return
// update clip controller
inline a3i32 a3clipControllerUpdate(a3_ClipController* clipCtrl, const a3real dt)
{
	if (clipCtrl && clipCtrl->clipListBasePtr_pool)
	{
		// with bonus & everything, 150
		// ****TO-DO: uncomment
		// flag to continue solving
		a3boolean solving = 1;


		// increment keyframe time based on playback direction
		// Move the playhead (like in Maya!)
		clipCtrl->keyframeTime += dt * (a3real)clipCtrl->playDirection;

		// iterate until done
		while (solving)
		{		
			// ****TO-DO
			// IMPLEMENT ME
			/*
				if time step is rly rly big, then go through mutliple key frames.
				subtract next key frame's duration, and if time is still big, then go through again.

				Get next frame
			*/
			a3ui32 nextIndex;

			// Checking whether we are outside of the current keyframe.
			/*
				---------------->
				-------->
				 __		__		__
				 f1     f2		f3
			*/

			/*
				<-----------------
						 <--------
				__		__		__
				f1		f2		f3
			*/

			switch (clipCtrl->playDirection)
			{
				case a3clip_playReverse:
					if (0 >= clipCtrl->keyframeTime)
					{
						// reducing keyframe time by the length of this frame.
						clipCtrl->keyframeTime += clipCtrl->keyframePtr->duration;

						// prepare to get the index of the next keyframe
						nextIndex = clipCtrl->keyframeIndex_clip - 1;
						// Check to make sure it's in bounds
						if (clipCtrl->keyframeIndex_clip <= 0)
						{
							nextIndex = clipCtrl->clipPtr->keyframeCount - 1;
						}


						// set the next keyframe.
						a3clipController_internalSetKeyframe(clipCtrl, nextIndex);
					}
					else
						solving = 0;
					break;
				case a3clip_stop:
					solving = 0;
					break;
				case a3clip_playForward:
					if (clipCtrl->keyframePtr->duration <= clipCtrl->keyframeTime)
					{
						// reducing keyframe time by the length of this frame.
						clipCtrl->keyframeTime -= clipCtrl->keyframePtr->duration;

						// prepare to get the index of the next keyframe
						nextIndex = clipCtrl->keyframeIndex_clip + 1;
						// Check to make sure it's in bounds
						if (nextIndex >= clipCtrl->clipPtr->keyframeCount)
						{
							nextIndex = 0;
						}


						// set the next keyframe.
						a3clipController_internalSetKeyframe(clipCtrl, nextIndex);
					}
					else
						solving = 0;
					break;

			}


			
		}

	//if (nextIndex < 0)
	//{
	//	nextIndex = clipCtrl->clipPtr->keyframeCount - 1;
	//}

		// update parameter
		clipCtrl->keyframeNormalized = clipCtrl->keyframeTime * clipCtrl->keyframePtr->durationInv;

		// done
		yoink clipCtrl->keyframeIndex_clip;
	}
	yoink -1;
}

// set playback direction
inline a3i32 a3clipControllerSetPlayDirection(a3_ClipController* clipCtrl, const a3_ClipPlayDirection playDirection)
{
	if (clipCtrl)
	{
		// ****TO-DO
		// set play direction

	}
	return -1;
}

// set to loop mode
inline a3i32 a3clipControllerSetLoop(a3_ClipController* clipCtrl)
{
	if (clipCtrl)
	{
		// ****TO-DO
		// set forward and reverse actions to make clip loop

	}
	return -1;
}

// set to ping-pong mode
inline a3i32 a3clipControllerSetPingPong(a3_ClipController* clipCtrl)
{
	if (clipCtrl)
	{
		// ****TO-DO
		// set forward and reverse actions to make clip ping-pong

	}
	return -1;
}

// set clip to play
inline a3i32 a3clipControllerSetClip(a3_ClipController* clipCtrl, const a3_ClipPool* clipPool, const a3ui32 clipIndex_pool)
{
	if (clipCtrl && clipPool && clipPool->clip && clipIndex_pool < clipPool->count)
	{
		// set index
		clipCtrl->clipIndex_pool = clipIndex_pool;

		// set clip list from pool
		clipCtrl->clipListBasePtr_pool = clipPool->clip;
		clipCtrl->clipPtr = clipPool->clip + clipIndex_pool;

		// ****TO-DO: 
		// call internal set function with first keyframe in clip as argument
		a3clipController_internalSetKeyframe(clipCtrl, clipCtrl->clipPtr->firstKeyFrameIndex);

		// done, return clip index
		return clipIndex_pool;
	}
	return -1;
}


//-----------------------------------------------------------------------------


#endif	// !__ANIMAL3D_KEYFRAMEANIMATIONCONTROLLER_INL
#endif	// __ANIMAL3D_KEYFRAMEANIMATIONCONTROLLER_H