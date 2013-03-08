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
        cg.WriteLine("STORE " + cg.NewAcc());
        Left.CodeGenCalvin(cg);

        switch (op)
        {
            case "+":
                cg.WriteLine("add right");
                break;
            case "-":
                cg.WriteLine("sub right");
                break;
            case "*":
                cg.WriteLine("mul right");
                break;
            case "/":
                cg.WriteLine("div right");
                break;
        }

        cg.FreeAcc();
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
