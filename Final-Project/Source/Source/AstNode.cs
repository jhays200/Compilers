
interface AstNode
{
    void GenCalvinCode(CodeGenerator cg);
    AstNode Left();
    AstNode Right();
}
