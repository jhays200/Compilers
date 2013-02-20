
class Symbol
{
public:
};

class Tokenizer
{

};

#include <map>
#include <utility>
#include <string>
#include <ostream>
#include "Tokenizer.hpp"
#include "CodeGen.hpp"

class SrParser
{
public:
	void Compile(ostream * out);
private:
	typedef void(*SrFunction)(SrParser & sr, int state);

	void AddIdentifier(std::string & name);

	//SrFunctions
	static void Shift(SrParser & sr, int state);
	static void Reduce(SrParser & sr, int state);
	static void Accept(SrParser & sr, int state);

	Tokenizer _tokenizer;
	CodeGen _codeGenerator;
	std::map<std::string,std::string> _symTable;
	std::map<std::pair<int,Symbol>, std::pair<SrFunction,int> > _srTable;
	std::ostream * _out;
};

#include <ostream>
#include "CodeGen.hpp"

class CodeGen
{
public:
	void CodeGenLLVM(std::ostream * out);
	void GenCalvinCode(std::ostream * out);

private:
	AstNode * _root;
	std::ostream * _out;
	unsigned int _lineNumber;
	unsigned int _constTableNumber;
	unsigned int _variableTableNumber;
	SymbolTable *_symbolTable;
};

class AstNode
{
public:
	virtual void CodeGenCalvin(CodeGen & cg) = 0;
protected:
	virtual AstNode * Left() = 0;
	virtual AstNode * Right() = 0;
};

class OpNode: public AstNode
{
public:
	virtual void CodeGenCalvin(CodeGen & cg);
protected:
	virtual AstNode * Left();
	virtual AstNode * Right();
};

class AssNode: public AstNode
{
public:
	virtual void CodeGenCalvin(CodeGen & cg);
protected:
	virtual AstNode * Left();
	virtual AstNode * Right();
};

class StatNode: public AstNode
{
public:
	virtual void CodeGenCalvin(CodeGen & cg);
protected:
	virtual AstNode * Left();
	virtual AstNode * Right();
};

class IfNode: public AstNode
{
public:
	virtual void CodeGenCalvin(CodeGen & cg);
protected:
	virtual AstNode * Left();
	virtual AstNode * Right();
};

class GotoNode: public AstNode
{
public:
	virtual void CodeGenCalvin(CodeGen & cg);
protected:
	virtual AstNode * Left();
	virtual AstNode * Right();
};

class IdNode: public AstNode
{
public:
	virtual void CodeGenCalvin(CodeGen & cg);
protected:
	virtual AstNode * Left();
	virtual AstNode * Right();
};

class NumNode: public AstNode
{
public:
	virtual void CodeGenCalvin(CodeGen & cg);
protected:
	virtual AstNode * Left();
	virtual AstNode * Right();
};


