needs io/kbd io/mouse text/clip comp/c comp/sig io/stream gr/buf gr/rdwr
unit emul/oberon

16 value megs
0 value leds
1 value console
0 value _rampointer
variable _reset
f"data/oberon.dsk" value diskfile

: _mouseupd ( xadr yadr -- buttons )
  mouseupdate mouse xy rot ! swap ! mouse buttons ;
annotatelast ( AnyPtr AnyPtr -- uint )

: _getnkc keyboard ?nkc not if idle 0 then ;
annotatelast ( -- uint )

: _nkc2char keyboard nkc>char ;
annotatelast ( uint -- uint )

: _ftruncate truncate ;
annotatelast ( AnyPtr -- uint )

: _set_pixel ( color y x -- ) rot >screencolor swap rot screen pixel! ;
annotatelast ( uint uint uint -- )

: risc_get_clip_pointer clipboard ;
annotatelast ( -- *uchar )

: risc_read_clipboard clipboardlen @ ;
annotatelast ( -- uint )

: risc_set_clipboard clipboardlen ! ;
annotatelast ( uint -- )

: risc_ensure_clipboard clipensure ;
annotatelast ( uint -- )

: _run_command c@+ interpret[] ;
annotatelast ( *uchar -- )

cc<< emul/oberon/dusk.c
cc<< emul/oberon/risc.c

:c void _oberon(int reset, int width, int height, uint megs, uint* ramptr,
                bool leds, bool console, Stream *disk_file) {
  STRUCT RISC *risc;
  uint timer, baseticks, newticks, elapsedms, nkc, cc;
  uint xx, yy, bb;

  timer = 0;
  baseticks = ticks();
  risc = risc_new(reset, width, height, ramptr, megs, leds, console);
  risc->disk.disk_file = disk_file;
  do {
    newticks = ticks();
    elapsedms = (newticks - baseticks) * uspertick / 1000;
    timer += elapsedms;
    baseticks += elapsedms * 1000 / uspertick;
    risc_set_time(risc, timer);
    bb = _mouseupd(&yy, &xx);
    risc_mouse_moved(risc, xx, (uint)height - 1 - yy);
    risc_mouse_button(risc, 1, (bb & 1) != 0);
    risc_mouse_button(risc, 3, (bb & 2) != 0);
    risc_mouse_button(risc, 2, (bb & 4) != 0);
    nkc = _getnkc();
    cc = _nkc2char(nkc);
	if (nkc && !cc) {
      switch(nkc) {
        case $a4: cc = 127; break;
        case $ab: cc = 19; break;
        case $ac: cc = 20; break;
        case $a9: cc = 17; break;
        case $aa: cc = 18; break;
        case $81: cc = 26; break;
        case $82: cc = 25; break;
        case $83: cc = 24; break;
        case $84: cc = 23; break;
        case $85: cc = 22; break;
        case $86: cc = 21; break;
      }
    }
    if (cc > 1024) {
      cc -= 1024;
    } else if (cc > 127) {
      cc = 0;
    }
    if (cc) {
      risc_keyboard_input(risc, cc);
    }
  } while (risc_run(risc, 10000));
}

: ocont
  _rampointer not if
    here to _rampointer
    megs 1024 * 1024 * allot
  then

  \ arguments for _oberon
  diskfile console leds _rampointer megs screen height screen width
  0 _reset @! _oberon ;

: obreak 1 _reset ! ocont ;
: oberon 2 _reset ! ocont ;
