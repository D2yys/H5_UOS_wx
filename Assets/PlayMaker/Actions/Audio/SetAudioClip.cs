// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Sets the Audio Clip played by the AudioSource component on a Game Object.")]
	public class SetAudioClip : ComponentAction<AudioSource>
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with the AudioSource component.")]
		public FsmOwnerDefault gameObject;

		[ObjectType(typeof(AudioClip))]
		[Tooltip("The AudioClip to set.")]
		public FsmObject audioClip;
		public FsmBool Addressable;

		public FsmString SoumName;
		public override void Reset()
		{
			gameObject = null;
			audioClip = null;
			
		}

		public override void OnEnter()
		{
			if (Addressable.Value == true) { 
			Addressables.LoadAssetAsync<AudioClip>(SoumName.ToString()).Completed += (hal) =>
			{
				var go = Fsm.GetOwnerDefaultTarget(gameObject);
				if (UpdateCache(go))
				{
					audio.clip = hal.Result;

				}				
				Finish();

			};
            }
            else { 
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(go))
			{
	            audio.clip = audioClip.Value as AudioClip;
				
			}
				Finish();
			}
			
		}

#if UNITY_EDITOR
	    public override string AutoName()
	    {
	        return ActionHelpers.AutoName(this, audioClip);
	    }
#endif

	}
}