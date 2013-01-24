#ifndef PARSER_CLASS
#define PARSER_CLASS

#include <list>
#include <sstream>
#include "Token.hpp"
#include "AstNodes.hpp"

class Parser
{
public:
	static iAstNode * Start(std::list<Token> & tokenList);
private:
	static iAstNode * Expr();
	static iAstNode * Factor();
	static iAstNode * Term();
	static cAstOperation * Addend();
	static bool GetNextToken();

	static Token _currToken;
	static std::list<Token>::iterator _nextToken;
};

#endif //PARSER_CLASS