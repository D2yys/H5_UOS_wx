// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.
using UnityEngine;
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Easy Save 2")]
	[Tooltip("Load values with Easy Save 2")]
	public class ES2LoadValue : FsmStateAction
	{
		[Tooltip("The prefix tag for the data we want to load. Not mandatory, as values below must have tags anyway")]
		public FsmString prefixTag = "";
		
		[RequiredField]
		[Tooltip("The name of the file our data is stored in.")]
		public FsmString saveFile = "defaultES2File.txt";
		
		[CompoundArray("Count", "Tag", "Value")]
		[RequiredField]
		[Tooltip("The unique tag for referencing.")]
		public FsmString[] tags;
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmVar[] values;

		public override void Reset()
		{
			tags = new FsmString[1];
			FsmString _val = new FsmString();	
			_val.Value = "My Value";
			tags[0] = _val;
			values = new FsmVar[1];
		}
		
		public override string ErrorCheck ()
		{
			for(int i = 0; i<tags.Length;i++){
					//string tag = tags[i].Value;
					FsmVar fsmVar = values[i];
						
					switch (fsmVar.Type) {
						case VariableType.Int:
						case VariableType.Float:
						case VariableType.Bool:
						case VariableType.Color:
						case VariableType.Quaternion:
						case VariableType.Rect:
						case VariableType.Vector2:
						case VariableType.Vector3:
						case VariableType.String:
							break;
						default:
						return "EasySave2 does not support "+ fsmVar.Type;
					}
			}
			 return "";
		}
		
		public override void OnEnter()
		{
			string file = saveFile.Value+"?tag="+prefixTag;
			
			for(int i = 0; i<tags.Length;i++){
				if(!tags[i].IsNone || !tags[i].Value.Equals("")) 
				{
					string tag = tags[i].Value;
					FsmVar fsmVar = values[i];
						
					string _fullFile = file;
					if (!tags[i].IsNone)
					{
						_fullFile = _fullFile+tag;
					}

					switch (fsmVar.Type) {
						case VariableType.Int:
						PlayMakerUtils.ApplyValueToFsmVar(this.Fsm,fsmVar,ES2.Load<int>(_fullFile));
							break;
						case VariableType.Float:
						PlayMakerUtils.ApplyValueToFsmVar(this.Fsm,fsmVar,ES2.Load<float>(_fullFile));
							break;
						case VariableType.Bool:
						PlayMakerUtils.ApplyValueToFsmVar(this.Fsm,fsmVar,ES2.Load<bool>(_fullFile));
							break;
						case VariableType.Color:
						PlayMakerUtils.ApplyValueToFsmVar(this.Fsm,fsmVar,ES2.Load<Color>(_fullFile));
							break;
						case VariableType.Quaternion:
						PlayMakerUtils.ApplyValueToFsmVar(this.Fsm,fsmVar,ES2.Load<Quaternion>(_fullFile));
							break;
						case VariableType.Rect:
						PlayMakerUtils.ApplyValueToFsmVar(this.Fsm,fsmVar,ES2.Load<Rect>(_fullFile));
							break;
						case VariableType.Vector2:
						PlayMakerUtils.ApplyValueToFsmVar(this.Fsm,fsmVar,ES2.Load<Vector2>(_fullFile));
							break;
						case VariableType.Vector3:
						PlayMakerUtils.ApplyValueToFsmVar(this.Fsm,fsmVar,ES2.Load<Vector3>(_fullFile));
							break;
						case VariableType.String:
						PlayMakerUtils.ApplyValueToFsmVar(this.Fsm,fsmVar,ES2.Load<string>(_fullFile));
							break;
						default:
							LogError("PlayerPrefsx does not support saving "+ fsmVar.Type);
							break;
					}
					
					
					
					
				}
			}
			
			Finish();
		}
		

	}
}