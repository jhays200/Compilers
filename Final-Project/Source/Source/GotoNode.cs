using System;


class GotoNode:IAstNode
{
	string _label;
	
	public GotoNode (string label)
	{
		_label = label;
	}
	
	public void CodeGenCalvin(CodeGenerator cg)
	{
		cg.WriteLine("JUMP {" + cg.GetLabelPlaceHolder(_label) + "}");
	}
	
    public IAstNode Left{get{return null;}}
    public IAstNode Right { get{return null;} }
}
