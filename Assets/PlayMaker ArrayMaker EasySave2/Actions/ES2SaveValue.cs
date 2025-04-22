// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Easy Save 2")]
	[Tooltip("Save values with Easy Save 2")]
	public class ES2SaveValue : FsmStateAction
	{
		[Tooltip("The prefix tag for the data we want to save. Not mandatory, as values below must have tags anyway")]
		public FsmString prefixTag = "";
		
		[RequiredField]
		[Tooltip("The name of the file our data is stored in.")]
		public FsmString saveFile = "defaultES2File.txt";
		
		[CompoundArray("Count", "Tag", "Value")]
		[RequiredField]
		[Tooltip("The unique tag for referencing.")]
		public FsmString[] tags;
		
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
			string file = saveFile.Value+"?tag="+prefixTag.Value;
				
			for(int i = 0; i<tags.Length;i++){
				if(!tags[i].IsNone || !tags[i].Value.Equals("")) 
				{
					string tag = tags[i].Value;
					FsmVar fsmVar = values[i];
					//string _name = fsmVar.variableName;

					string _fullFile = file;
					if (!tags[i].IsNone)
					{
						_fullFile = _fullFile+tag;
					}

					switch (fsmVar.Type) {
						case VariableType.Int:
						ES2.Save(fsmVar.IsNone ? 0 : (int)PlayMakerUtils.GetValueFromFsmVar(this.Fsm,fsmVar),_fullFile);
							break;
						case VariableType.Float:
						ES2.Save(fsmVar.IsNone ? 0f : (float)PlayMakerUtils.GetValueFromFsmVar(this.Fsm,fsmVar) ,_fullFile);
							break;
						case VariableType.Bool:
						ES2.Save(fsmVar.IsNone ? false : (bool)PlayMakerUtils.GetValueFromFsmVar(this.Fsm,fsmVar),_fullFile);
							break;
						case VariableType.Color:
						ES2.Save(fsmVar.IsNone ? Color.black : (Color)PlayMakerUtils.GetValueFromFsmVar(this.Fsm,fsmVar),_fullFile);
							break;
						case VariableType.Quaternion:
						ES2.Save(fsmVar.IsNone ? Quaternion.identity : (Quaternion)PlayMakerUtils.GetValueFromFsmVar(this.Fsm,fsmVar),_fullFile);
							break;
						case VariableType.Rect:
						ES2.Save(fsmVar.IsNone ? new Rect(0f,0f,0f,0f) : (Rect)PlayMakerUtils.GetValueFromFsmVar(this.Fsm,fsmVar),_fullFile);
							break;
						case VariableType.Vector2:
						ES2.Save(fsmVar.IsNone ? Vector2.zero : (Vector2)PlayMakerUtils.GetValueFromFsmVar(this.Fsm,fsmVar),_fullFile);
							break;
						case VariableType.Vector3:
						ES2.Save(fsmVar.IsNone ? Vector3.zero : (Vector3)PlayMakerUtils.GetValueFromFsmVar(this.Fsm,fsmVar),_fullFile);
							break;
						case VariableType.String:
						ES2.Save(fsmVar.IsNone ? "" : (string)PlayMakerUtils.GetValueFromFsmVar(this.Fsm,fsmVar),_fullFile);
							break;
						default:
							LogError("EasySave2 does not support saving "+ fsmVar.Type);
							break;
					}
					
				}
			}
			
			Finish();
		}
		

	}
}