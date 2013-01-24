#include "AstNodes.hpp"
#include <sstream>

cAstLiteral::cAstLiteral(int i):_i(i)
{}

std::string cAstLiteral::GenerateAsm(std::ostream & out, int & ip)
{
	std::ostringstream tmp;
	tmp << _i;
	return tmp.str();
}

iAstNode * cAstLiteral::Left()
{
	return 0;
}

iAstNode * cAstLiteral::Right()
{
	return 0;
}

cAstOperation::cAstOperation(iAstNode * left, char op, iAstNode * right):
	_op(op), _left(left), _right(right)
{}

cAstOperation::~cAstOperation()
{
	delete Left();
	delete Right();
}

std::string cAstOperation::GenerateAsm(std::ostream & out, int & ip)
{
	using std::string;
	using std::ostringstream;
	
	string left = Left()->GenerateAsm(out, ip);
	string right = Right()->GenerateAsm(out,ip);
	
	ostringstream retValue;
	
	switch(_op)
	{
	case '+':
		out << "\t%" << ++ip << " = add i32 " << right << ", " << left << '\n';
		break;
	case '*':
		out << "\t%" << ++ip << " = mul i32 " << left << ", " << right << '\n';
		break;
	case '/':
		out << "\t%" << ++ip << " = sdiv i32 " << left << ", " << right << '\n';
		break;
	default:
		out << "Invalid Operation - Symbol not found\n";
		break;
	}
	
	retValue << '%' << ip;
	return retValue.str();
}

iAstNode * cAstOperation::Left()
{
	return _left;
}

iAstNode * cAstOperation::Right()
{
	return _right;
}