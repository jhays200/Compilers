#include <list>
#include "Token.hpp"
#include <iostream>
#include <sstream>
#include <string>
#include <memory>
#include <map>
#include <cctype>
#include "Parser.hpp"

void OutputAssembly(iAstNode * treeTop, std::ostream & output);
bool ContinueProgram();
std::string GetInput();
std::auto_ptr<std::list<Token> > GetTokens();
std::map<char, Token> OperatorDict();
void CheckValid(Token & workingToken, char c);

int main()
{
	using namespace std;

	do
	{
		try
		{
			//Lexing
			auto_ptr<list<Token> > tokens = GetTokens();
			cout << "Tokens Passed to the Parser\n";
			for(list<Token>::iterator i = tokens->begin(); i != tokens->end(); ++i)
				cout << "\t" << i->type << "\t" << i->value << "\n";
			cout.flush();

			try
			{
				stringstream out;
				iAstNode * treeTop = Parser::Start(*tokens);

				OutputAssembly(treeTop, out);
				cout << "\nParser Contents: \n"
					<< out.rdbuf() << endl;

				delete treeTop;
			}
			catch(const char * error)
			{
				cout << error << endl;
			}
		}
		catch(string error)
		{
			cout << error << endl;
		}
	} while(ContinueProgram());

	return 0;
}

void OutputAssembly(iAstNode * treeTop, std::ostream & output)
{
	int ip = 0;

	output 	<< "@.str = private constant [13 x i8] c\"result = %d\\0A\\00\"\n\n"
				<< "define i32 @main() nounwind {\n";

	treeTop->GenerateAsm(output, ip);

	output 	<< "\n\t%call = call i32 (i8*, ...)*"
				<< " @printf(i8* getelementptr inbounds "
				<<	"([13 x i8]* @.str, i32 0, i32 0), "
				<< " i32 %" << ip << ")\n"
				<< "\tret i32 0\n"
				<< "}\n"
				<< "\n"
				<< "declare i32 @printf(i8*, ...)\n";
}

bool ContinueProgram()
{
	using namespace std;
	char c;
	cout << "Would you like to continue Y/N?";

	cin.clear();
	cin.ignore(cin.rdbuf()->in_avail());
	cin >> c;
	cin.ignore();

	return 'Y' == c || 'y' == c;
}

std::string GetInput()
{
	using namespace std;

	string line;

	cout << "Please Enter Arithmetic Expression:\n\t";
	cout.flush();
	
	getline(cin, line);
	cin.clear();
	cin.ignore(cin.rdbuf()->in_avail());

	return line;
}

std::auto_ptr<std::list<Token> > GetTokens()
{
	using namespace std;

	static map<char,Token> operatorDict = OperatorDict();
	auto_ptr<list<Token> > tokens(new std::list<Token>);
	string line = GetInput();
	stringstream tokenBuffer;
	bool clearBuff = false;
	bool opFound = false;
	Token workingToken;

	for(string::iterator i = line.begin(); i != line.end(); ++i)
	{
		if(*i == ' ')
			continue;

		switch(*i)
		{
		case '+':
		case '-':
		case '*':
		case '/':
		case '=':
		case '(':
		case ')':
			opFound = true;
			//fallthrough
		case '\n':
			clearBuff = true;
			break;
		default:
			CheckValid(workingToken, *i);
			tokenBuffer << *i;
			break;
		}

		if(clearBuff)
		{
			clearBuff = false;

			if(workingToken.type != Token::Eof)
			{
				workingToken.value = tokenBuffer.str();
				tokenBuffer.str("");
				tokenBuffer.clear();

				tokens->push_back(workingToken);
				workingToken = Token();
			}
		}

		if(opFound)
		{
			opFound = false;
			tokens->push_back(operatorDict[*i]);
		}
	}

	if(workingToken.type != Token::Eof)
	{
		workingToken.value = tokenBuffer.str();
		tokens->push_back(workingToken);
	}

	tokens->push_back(Token());

	return tokens;
}

std::map<char, Token> OperatorDict()
{
	using std::map;
	using std::string;

	map<char,Token> tmp;
	tmp['+'] = Token(Token::Plus, string("+"));
	tmp['-'] = Token(Token::Minus, string("-"));
	tmp['*'] = Token(Token::Times, string("*"));
	tmp['/'] = Token(Token::Divide, string("/"));
	tmp['='] = Token(Token::Equal, string("="));
	tmp['('] = Token(Token::L_Paren, string("("));
	tmp[')'] = Token(Token::R_Paren, string(")"));
	
	return tmp;
}

void CheckValid(Token & workingToken, char c)
{
	if(workingToken.type == Token::Eof)
	{
		if(isdigit(c))
			workingToken.type = Token::Num;

		if(isalpha(c))
			workingToken.type = Token::Id;
	}

	if(workingToken.type == Token::Num
		&& isalpha(c))
		throw std::string("Invalid Number Character: ") + c;
}

