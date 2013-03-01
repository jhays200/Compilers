

interface IAstNode
{
    void CodeGenCalvin(CodeGenerator cg);
    IAstNode Left{get;}
    IAstNode Right { get; }
}
