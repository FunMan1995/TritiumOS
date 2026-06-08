needs io/kbd
unit bench/kbd

0 value lastkey
0 value lastrepeat

: keybench ( kbd -- )
  0 to lastkey 0 to lastrepeat
  begin
    begin
      idle dup keyboard <> if keyboard ?nkc if 2drop exit then then
      dup ?nkc until ( kbd nkc )
    dup lastkey = if
      doto lastrepeat 1+ | else dup to lastkey 0 to lastrepeat then
    ."NKC: " dup .x spc>
    ."pressed: " over pressed .x spc>
    ."mods: " over mods .x spc>
    CodeMask and case
      BS = of ."Backspace key" endof
      9 = of ."Tab key" endof
      CR = of ."Return key" endof

      SPC = of ."Space key" endof
      ESC = of ."Escape key" endof
      Insert = of ."Insert key" endof
      Delete = of ."Delete key" endof

      ArrowUp = of ."Up key" endof
      ArrowDown = of ."Down key" endof
      ArrowLeft = of ."Left key" endof
      ArrowRight = of ."Right key" endof

      Home = of ."Home key" endof
      End = of ."End key" endof
      PageUp = of ."PageUp key" endof
      PageDown = of ."PageDown key" endof

      F1 = of ."F1 key" endof
      F2 = of ."F2 key" endof
      F3 = of ."F3 key" endof
      F4 = of ."F4 key" endof
      F5 = of ."F5 key" endof
      F6 = of ."F6 key" endof
      F7 = of ."F7 key" endof
      F8 = of ."F8 key" endof
      F9 = of ."F9 key" endof
      F10 = of ."F10 key" endof

      33 126 within? of ."Printable key " r@ emit endof
      drop ."Unknown keycode " .
    endcase nl>
    lastrepeat 2 > until drop ;
