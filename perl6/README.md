## macOS 
- Install Homebrew by opening the Terminal and running the following command: 
/usr/bin/ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"  
 
2. Install Rakudo, the Perl6 compiler, using Homebrew by running the following command: 
- brew install rakudo-star  
 
3. Install the required Perl6 modules by running the following command: 
- zef install DBIish HTTP::UserAgent Text::CSV Cro  
 
4. Save the Perl6 code into a file, e.g. csv_parser.p6. 
Run the Perl6 code by opening the Terminal and running the following command: 
- perl6 csv_parser.p6  
 
## Linux 
Install Rakudo, the Perl6 compiler, by following the instructions on the Rakudo website: https://rakudo.org/how-to-get-rakudo/ 
Install the required Perl6 modules by running the following command: 
- zef install DBIish HTTP::UserAgent Text::CSV Cro  
 
3. Save the Perl6 code into a file, e.g. csv_parser.p6. 
Run the Perl6 code by opening the Terminal and running the following command: 
- perl6 csv_parser.p6  
 
## Windows 
Install Rakudo, the Perl6 compiler, by following the instructions on the Rakudo website: https://rakudo.org/how-to-get-rakudo/ 
Install the Strawberry Perl distribution, which includes a C compiler needed for some Perl6 modules, by following the instructions on the 
### Strawberry Perl website: http://strawberryperl.com/ 
Install the required Perl6 modules by running the following command in the Command Prompt: 
- zef install DBIish HTTP::UserAgent Text::CSV Cro  
 
4. Save the Perl6 code into a file, e.g. csv_parser.p6. 
Run the Perl6 code by opening the Command Prompt and running the following command: 
- perl6 csv_parser.p6  

# DOCS from Rakudo Bundle
## Perl 6 is best used from the command line or inside an editor such as Atom,
- Emacs or Vim etc. It is not a full Mac GUI type app.

You may need to relax security permissions under "System Prefs>Security &
Privacy>General" to allow the use of unsigned third party apps for some uses.

1. Drag the 'Rakudo' folder onto the '/Applications' folder. Note this 
is the root Applications directory and other locations won't work.

2. Drag the 'Docs' folder somewhere.

3. In a Terminal window add Rakudo Star to your path:

   - export PATH=$PATH:/Applications/Rakudo/bin:/Applications/Rakudo/share/perl6/site/bin

   Test with:

   - perl6 -e 'say "Hello, world!";'

4. Add the 'export' line above to .bash_profile or similar to persist.

5. Useful commands are 'p6doc' and 'zef'.

6. See also the documents in the 'Docs' folder and http://perl6.org

