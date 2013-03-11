using System;

class LabelNode: IAstNode
{
    int _labelPlaceholder;
    IAstNode _ulst;

    public LabelNode(int labelPlaceholder, IAstNode ulst)
    {
        _labelPlaceholder = labelPlaceholder;
        _ulst = ulst;
    }

    public void CodeGenCalvin(CodeGenerator cg)
    {
        cg.SetPhysicalLabelLocation(_labelPlaceholder);
		_ulst.CodeGenCalvin(cg);
    }

    public IAstNode Left 
    {
        get
        {
            return null;
        }
    }
    public IAstNode Right
    {
        get
        {
            return _ulst;
        }
    }
}
