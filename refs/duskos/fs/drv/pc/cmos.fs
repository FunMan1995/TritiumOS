needs asm/x86
unit drv/pc/cmos

code cmos@ ( regnum -- val )
  cli, al $70 imm) out, al $71 imm) in, sti, ret,
