using System;
using System.Collections.Generic;

class IfNode:IAstNode
{
    IAstNode _left;
    IAstNode _right;

    public IfNode(IAstNode rel, IAstNode ucdst)
    {
        _left = rel;
        _right = ucdst;
    }

    public void CodeGenCalvin(CodeGenerator cg)
    {
        Left.CodeGenCalvin(cg);
        cg.WriteLine("SKIPT");
        int labelNumber = cg.NewAroundLabel();
        cg.WriteLine("JUMP {" + labelNumber + "}");
        Right.CodeGenCalvin(cg);
        cg.SetPhysicalLabelLocation(labelNumber);
    }

    public IAstNode Left 
    {
        get
        {
            return _left;
        }
    }
    public IAstNode Right
    {
        get
        {
            return _right;
        }
    }
}