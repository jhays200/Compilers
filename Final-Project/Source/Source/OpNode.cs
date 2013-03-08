using System;


class OpNode: IAstNode
{
    private IAstNode _left;
    private IAstNode _right;
    private string op;

    public OpNode(IAstNode left, string op, IAstNode right)
    {
        _left = left;
        this.op = op;
        _right = right;
    }

    public void CodeGenCalvin(CodeGenerator cg)
    {
        
        Right.CodeGenCalvin(cg);
        cg.WriteLine("STORE " + cg.NewAccu());
        Left.CodeGenCalvin(cg);

        switch (op)
        {
            case "+":
                cg.WriteLine("ADD " + cg.GetAccu());
                break;
            case "-":
                cg.WriteLine("SUB " + cg.GetAccu());
                break;
            case "*":
                cg.WriteLine("MUL " + cg.GetAccu());
                break;
            case "/":
                cg.WriteLine("DIV " + cg.GetAccu());
                break;
            case "=":
                cg.WriteLine("EQ " + cg.GetAccu());
                break;
            case ">":
                cg.WriteLine("GR " + cg.GetAccu());
                break;
            case "<":
                cg.WriteLine("LESS " + cg.GetAccu());
                break;
        }

        cg.FreeAccu();
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
