#include <stdio.h>
extern char *bootcmd;
extern int stderremit;
void inlinecmd(char *cmd);
void setupprompt();
int duskopts(int argc, char **argv);
#define _printf(...) fprintf(stderremit ? stderr : stdout, __VA_ARGS__)
