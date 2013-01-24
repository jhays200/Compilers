#include "Token.hpp"

//constants
const Token::Type Token::Plus = (Type)0;
const Token::Type Token::Minus = (Type)1;
const Token::Type Token::Times = (Type)2;
const Token::Type Token::Divide = (Type)3;
const Token::Type Token::Num = (Type)4;
const Token::Type Token::Id = (Type)5;
const Token::Type Token::Equal = (Type)6;
const Token::Type Token::Eof = (Type)7;
const Token::Type Token::L_Paren = (Type)8;
const Token::Type Token::R_Paren = (Type)9;

//Constructors
Token::Token():
type(Token::Eof)
{}

Token::Token(Type type, std::string value):
type(type), value(value)
{}

Token::Token(const Token & token)
:type(token.type), value(token.value)
{}

Token & Token::operator=(const Token & rhs)
{
	if(this != &rhs)
	{
		type = rhs.type;
		value = rhs.value;
	}

	return *this;
}