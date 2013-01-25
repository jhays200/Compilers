#include "Parser.hpp"
//#include <iostream>
//using std::cout;
//using std::endl;

Token Parser::_currToken = Token();
std::list<Token>::iterator Parser::_nextToken;

//Start -> id = Expr
iAstNode * Parser::Start(std::list<Token> & tokenList)
{
	_nextToken = tokenList.begin();
	_currToken = *_nextToken;
	++_nextToken;

	if(_currToken.type == Token::Id)
	{

		if(GetNextToken() && _currToken.type == Token::Equal)
		{
			if(!GetNextToken())
				throw "No expression to evaluate";
			return Expr();
		}
		else
			throw "Missing the '=' sign";
	}

	throw "Invalid Id";

	if(_currToken.type == Token::Eof)
		throw "S-> id = Expr does not parse entire string";

	return 0;
}
//Expr -> Factor Addend
iAstNode * Parser::Expr()
{
	iAstNode * factor = 0;
	cAstOperation * addend = 0;

	if(factor = Factor())
	{
		addend = Addend();
	}
	else
		throw "No Factor Found";

	if(addend)
	{
		addend->_right = factor;
		return addend;
	}
	
	return factor;
}

//Factor -> Term * Factor
//Factor -> Term / Factor
//Factor -> Term
iAstNode * Parser::Factor()
{
	iAstNode * term = 0;

	if (term = Term())
	{
		char opChar = 0;

		if(_currToken.type == Token::Times)
		{
			if(!GetNextToken())
			{
				throw "No Number Found to Multiply";
			}
			opChar = '*';
		}
		
		if (_currToken.type == Token::Divide)
		{
			if(!GetNextToken())
			{
				throw "No Number Found to Divide";
			}

			opChar = '/';
		}

		if(opChar)
		{
			cAstOperation * node = new cAstOperation(term, opChar, Factor());

			if(node->_right == 0)
				throw "Invalid Factor found";

			return node;
		}
		
		return term;
	}

	throw "Invalid Factor Found";

	return 0;
}


//Term -> num
//Term -> ( Expr )
iAstNode * Parser::Term()
{
	if(_currToken.type == Token::Num)
	{
		std::istringstream value(_currToken.value);
		int lit = 0;
		value >> lit;

		GetNextToken();
		
		return new cAstLiteral(lit);
	}
	
	if(_currToken.type == Token::L_Paren)
	{
		iAstNode * expr = 0;

		if(!GetNextToken())
			throw "Invalid Expression to Right of (";

		expr = Expr();

		if(expr == 0)
			throw "Invalid Expression Given";
			
		if(_currToken.type == Token::R_Paren)
		{
			GetNextToken();
			return expr;
		}
		else
			throw "No matching Right Paren";
	}
	
	return false;
}

//Addend -> + Factor Addend
//Addend -> - Factor Addend
//Addend -> lambda
cAstOperation * Parser::Addend()
{
	char opChar = 0;

	if(_currToken.type == Token::Plus)
		opChar = '+';

	if(_currToken.type == Token::Minus)
		opChar = '-';

	if(opChar)
	{
		if(!GetNextToken())
			throw "Missing addend to the right of the plus or minus";

		iAstNode * factor = Factor();
		cAstOperation * addend = Addend();

		//Subtraction shouldn't exist, so multiply by -1 instead
		if(opChar == '-')
		{
			iAstNode * negationFactor = new cAstOperation(new cAstLiteral(-1), '*', factor);
			factor = negationFactor;
		}

		if(addend)
		{
			addend->_right = factor;
			factor = addend;
		}

		return new cAstOperation(factor, '+', 0);
	}

	return 0;
}


bool Parser::GetNextToken()
{
	if(_nextToken->type == Token::Eof)
		return false;

	_currToken = *_nextToken++;
	return true;
}
	
