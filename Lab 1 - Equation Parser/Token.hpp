#ifndef TOKEN_CLASS
#define TOKEN_CLASS

#include <string>

class Token
{
public:
	//constants and definitions
	typedef unsigned int Type;
	static const Type Plus;
	static const Type Minus;
	static const Type Times;
	static const Type Divide;
	static const Type Num;
	static const Type Id;
	static const Type Equal;
	static const Type Eof;
	static const Type L_Paren;
	static const Type R_Paren;

	//Constructor
	Token();
	Token(Type type, std::string value = std::string());
	Token(const Token & token);
	Token & operator=(const Token & rhs);

	//members
	Type type;
	std::string value;
};

#endif //TOKEN_CLASS