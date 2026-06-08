needs lib/struct gr/buf drv/pc/vga lib/fmt
unit drv/pc/vesa

\ TODO: for now, we only support 16bpp modes

struct VBEInfo {
  leint sig ;
  leshort version ;
  leint oem capabilities videomodes ;
  leshort videomem softwarerev ;
  leint vendor productname productrev ;
}

struct Mode {
  leshort attributes ;
  uchar windowa windowb ;
  leshort granularity windowsize segmenta segmentb ;
  leint winfuncptr ;
  leshort vbepitch vbewidth vbeheight ;
  uchar wchar ychar planes bpp banks memorymodel banksize imagepages _pad ;
  uchar redmask redpos greenmask greenpos bluemask bluepos _pad pad ;
  uchar directcolorattributes ;
  leint framebuffer ;
  leint offscreenoff ;
  leshort offscreensize ;
}
Mode typesz const MODESZ

: linear? attributes $80 and bool ;

: _err abort"VESA error" ;
: _assert not if _err then ;

\ Transform a segment+offset into a 32-bit address
: seg>32 ( n -- n )
  dup 12 rshift $ffff0 and swap $ffff and + ;

$4e00 const VESASTRUCT

: _info ( -- info )
  $32454256 VESASTRUCT ! \ write VBE2 at the start of the struct
  0 0 $4f00 int10h ( bx ax )
  $4f = _assert drop ( info )
  VESASTRUCT dup sig $41534556 ( "VESA" ) = _assert ;

: .vesa _info >r \ V1=info
  r@ version .f"Version: %w\n"
  r@ capabilities .f"Capabilities: %x\n"
  r@ videomem .f"Video memory (*64K): %w\n"
  r@ softwarerev .f"Software rev: %w\n"
  r@ vendor seg>32 .f"Vendor: %z\n"
  r@ productname seg>32 .f"Product name: %z\n"
  r> productrev seg>32 .f"Product rev: %z\n" ;

: _modeinfo ( mode -- info )
  ( cx ) 0 $4f01 int10h ( bx ax )
  $4f = _assert drop VESASTRUCT ;

: _.mode ( mode -- )
  dup >r _modeinfo >r \ V1=modeid V2=modeinfo
  r@ linear?
  r@ bpp r@ vbeheight r> vbewidth
  r> .f"|%w %dx%dx%d" ( linear ) if '*' emit then spc> ;

\ We have to read all mode IDs before we call int10h because that list might be
\ overwritten by int10h.
: .vesamodes
  _info videomodes seg>32 >r \ V1=a
  -1 ( sentinel ) begin
    doto V1 dup 2+ | w@ dup $ffff = until ( -1 ... $ffff )
  drop rdrop
  begin ( -1 ... ) dup 0>= while _.mode repeat ( -1 ) drop ;

$111 const DEFAULT_VESAMODE

\ Here, we deal only with linear modes
create _curmode MODESZ allot
: vesamode ( mode -- )
  dup _modeinfo dup linear? _assert ( mode info )
  _curmode MODESZ cmove ( mode )
  0 swap $4000 or $4f02 int10h ( bx ax )
  $4f = _assert drop ( )
  16BPP RGB565 _curmode bi vbewidth | vbeheight
  _curmode bi vbepitch | framebuffer screen configurebuf
  1 screen to flipY ;

\ There used to be a VESA 1.2 implementation, but I lost the hardware and I
\ refactored the graphic stack in a way that broke it. When re-implementing it,
\ another approach will be needed. Maybe something like an in-memory buffer
\ proxy that copies its damaged parts to the different banks.
