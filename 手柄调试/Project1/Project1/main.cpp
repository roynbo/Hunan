#include<iostream>
#include<string>
#include<vector>
#include<algorithm>
#include<cstring>

using namespace std;
bool exist(vector<vector<char>>& board, string word);
int main()
{
	vector<vector<char>>vec;
	vector<char>temp;
	temp.push_back('a');
	temp.push_back('b');
	vec.push_back(temp);
	temp.clear(); 
	temp.push_back('c');
	temp.push_back('d');
	vec.push_back(temp);
	if (exist(vec, "abcd"))
		cout << "Y";
	else
		cout << "N";
	system("pause");
	return 0;
}

bool exist(vector<vector<char>>& board, string word) {
	int num = word.size();
	int i = 0;
	for (; i<word.size(); i++)
	{
		int j = 0;
		for (; j<board.size(); j++)
		{
			int k = 0;
			for (; k<board[j].size(); k++)
			{
				if (word[i] == board[j][k])
				{
					board[j].erase(board[j].begin() + k);
					num--;
					break;
				}
			}
		}

	}
	if (num == 0)
		return true;
	else
		return false;
}