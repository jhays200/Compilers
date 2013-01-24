#pragma once
#include <iostream>

class iAstNode
{
public:
	virtual ~iAstNode(){}
	virtual std::string GenerateAsm(std::ostream & out, int & ip) = 0;
protected:
	virtual iAstNode * Left() = 0;
	virtual iAstNode * Right() = 0;
};

class cAstLiteral: public iAstNode
{
public:
	cAstLiteral(int i);
	virtual std::string GenerateAsm(std::ostream & out, int & ip);
protected:
	virtual iAstNode * Left();
	virtual iAstNode * Right();
private:
	int _i;
};

class cAstOperation: public iAstNode
{
public:
	cAstOperation(iAstNode * left, char op, iAstNode * right);
	virtual std::string GenerateAsm(std::ostream & out, int & ip);
	iAstNode * _left;
	iAstNode * _right;
protected:
	virtual iAstNode * Left();
	virtual iAstNode * Right();
	
private:
	char _op;
};