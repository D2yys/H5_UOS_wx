// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Audio)]
    [Tooltip("Plays an Audio Clip at a position defined by a Game Object or Vector3. If a position is defined, it takes priority over the game object. This action doesn't require an Audio Source component, but offers less control than Audio actions.")]
    public class PlaySound : FsmStateAction
    {
        public FsmOwnerDefault gameObject;

        public FsmVector3 position;
        public FsmBool Addressable;
        public FsmString SoumName;
        //是否加载后释放
        //public FsmBool Release;
        [RequiredField]
        [Title("Audio Clip")]
        [ObjectType(typeof(AudioClip))]
        public FsmObject clip;

        [HasFloatSlider(0, 1)]
        public FsmFloat volume = 1f;

        public FsmString BoolName;

        public override void Reset()
        {
            gameObject = null;
            position = new FsmVector3 { UseVariable = true };
            clip = null;
            volume = 1;
            Addressable = true;
            //Release = true;
            BoolName = "Soundbool";
        }

        public override void OnEnter()

        {
            var numLives = FsmVariables.GlobalVariables.GetFsmBool(BoolName.Value);

            bool soundbool;

            soundbool = numLives.Value;
            
            if(soundbool!=true )
            {
                
                Finish();
                return;
            }


            if (Addressable.Value == true)
            {
                Addressables.LoadAssetAsync<AudioClip>(SoumName.ToString()).Completed += (hal) =>
                {
                    var audioClip = hal.Result;

                    if (audioClip == null)
                    {
                        LogWarning("Missing Audio Clip!");
                        return;
                    }

                    if (!position.IsNone)
                    {
                        AudioSource.PlayClipAtPoint(audioClip, position.Value, volume.Value);
                    }
                    else
                    {
                        var go = Fsm.GetOwnerDefaultTarget(gameObject);
                        if (go == null)
                        {
                            return;
                        }

                        AudioSource.PlayClipAtPoint(audioClip, go.transform.position, volume.Value);
                    }
                    //if (Release.Value == true) {
                    //    Addressables.Release(hal);
                    //}
                    Finish();
                };
               
            }
            else
            {
                DoPlaySound();
                Finish();
            }
        }

        void DoPlaySound()
        {
            var audioClip = clip.Value as AudioClip;

            if (audioClip == null)
            {
                LogWarning("Missing Audio Clip!");
                return;
            }

            if (!position.IsNone)
            {
                AudioSource.PlayClipAtPoint(audioClip, position.Value, volume.Value);
            }
            else
            {
                var go = Fsm.GetOwnerDefaultTarget(gameObject);
                if (go == null)
                {
                    return;
                }

                AudioSource.PlayClipAtPoint(audioClip, go.transform.position, volume.Value);
            }
        }

#if UNITY_EDITOR
        public override string AutoName()
        {
            return ActionHelpers.AutoName(this, clip);
        }
#endif

    }
}