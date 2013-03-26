//
//  main.cpp
//  unix_task_by_silent
//
//  Created by Deliany Delirium on 12.03.13.
//  Copyright (c) 2013 Clear Sky. All rights reserved.
//

#include <fstream>
#include <iostream>
#include <vector>
#include <map>

using namespace std;

struct convertWord {
    void operator()(char& c)
    {
        if(c == ' ')
        {
            c = '\n';
        }
        else
        {
            c = toupper((unsigned char)c);
        }
    }
};

struct convertWords {
    void operator()(string& str)
    {
        for_each(str.begin(), str.end(), convertWord());
    }
};

bool compareWords(string w1, string w2)
{
    return w1 < w2;
}

bool compareWordFrequency(pair<string,int> w1, pair<string,int> w2)
{
    return w1.second < w2.second;
}

ostream& operator << ( ostream& out,
                           const pair<string, int>& rhs )
{
    out << rhs.first << " : " << rhs.second;
    return out;
}


int main ( int argc, char *argv[] )
{
    if ( argc != 2 )
    { 
        cout<<"usage: "<< argv[0] <<" <filename>";
    }
    else
    {
        ifstream file ( argv[1] );
        if ( !file.is_open() )
        {
            cout << "Could not open file\n";
        }
        else
        {
            vector<string> words;
            string word;
            while(file >> word)
            {
                words.push_back(word);
            }
            
            // 1. convert all letters to upper case and replace spaces to new lines
            for_each(words.begin(), words.end(), convertWords());
            
            // 2. sort the result
            sort(words.begin(),words.end(), compareWords);
            
            // 3. count unique words
            map<string, int> wordCount;
            for (vector<string>::iterator it = words.begin(); it != words.end(); ++it) {
                ++wordCount[*it];
            }
            
            vector<pair<string,int> > myVec(wordCount.begin(), wordCount.end());
            
            
            // 4. sort the result reversive
            sort(myVec.rbegin(), myVec.rend(), compareWordFrequency);
            
            // 5. print head 10 records
            int elem = 0;
            for (vector<pair<string, int> >::iterator it = myVec.begin(); it != myVec.end(); ++it)
            {
                cout << it->first <<" : "<< it->second << endl;
                ++elem;
                if (elem == 10) {
                    break;
                }
            }
        }
    }
}

