unit drv/usb/const

16 const Ndeveps \ max nb. of endpoints per device
16 const Nconf   \ max. configurations per usb device
16 const Niface  \ max. interfaces per configuration
\ request type
0 7 lshift const Rh2d \ direction
1 7 lshift const Rd2h
0 5 lshift const Rstd \ types
2 5 lshift const Rvendor
1 5 lshift const Rclass
0 const Rdev \ recipients
1 const Riface
2 const Rep \ endpoint
3 const Rother

\ standard requests
consts
  0 Rgetstatus \
  1 Rclearfeature \
  3 Rsetfeature \
  5 Rsetaddress \
  6 Rgetdesc \
  7 Rsetdesc \
  8 Rgetconf \
  9 Rsetconf \
  10 Rgetiface \
  11 Rsetiface \
  12 Rsynchframe \
  $81 Rgetcur \
  $82 Rgetmin \
  $83 Rgetmax \
  $84 Rgetres \
  1 Rsetcur \
  2 Rsetmin \
  3 Rsetmax \
  4 Rsetres

enum Fullspeed Lowspeed Highspeed Nospeed
enum Dconfig Denabled Ddetach Dreset

\ device classes
consts
  0 Clnone \
  1 Claudio \
  2 Clcomms \
  3 Clhid \
  7 Clprinter \
  8 Clstorage \
  9 Clhub \
  10 Cldata

\ EP direction
0 const Ein
1 const Eout
2 const Eboth

\ transfer types. Values are fixed in protocol
1 const Tctl       \ wr req + rd/wr data + wr/rd sts
2 const Tiso       \ stream rd or wr (real time)
3 const Tbulk      \ stream rd or wr
4 const Tintr      \ msg rd or wr

\ standard descriptor sizes
consts
  18 Ddevlen \
  9 Dconflen \
  9 Difacelen \
  7 Deplen \
  9 Dhublen

\ descriptor types
consts
  1 Ddev \
  2 Dconf \
  3 Dstr \
  4 Diface \
  5 Dep \
  $22 Dreport \
  $24 Dfunction \
  $23 Dphysical \
  $29 Dhub

\ Port/device state
enum Pdisabled Pattached Pconfiged

\ Port status and status change bits
consts
  $0001 PSpresent \
  $0002 PSenable \
  $0004 PSsuspend \
  $0008 PSovercurrent \
  $0010 PSreset \
  $0100 PSpower \
  $0200 PSslow \
  $0400 PShigh

$10000 const PSstatuschg \ PSpresent changed
$20000 const PSchange    \ PSenable changed

100 const Powerdelay \ after powering up ports (ms)

\ hub class feature selectors
consts
  0 Fhublocalpower \
  1 Fhubovercurrent \
  0 Fportconnection \
  1 Fportenable \
  2 Fportsuspend \
  3 Fportovercurrent \
  4 Fportreset \
  8 Fportpower \
  9 Fportlowspeed \
  16 Fcportconnection \
  17 Fcportenable \
  18 Fcportsuspend \
  19 Fcportovercurrent \
  20 Fcportreset \
  22 Fportindicator

\ Delays, timeouts (ms)
100 const Spawndelay \ how often may we re-spawn a driver
500 const Connectdelay \ how much to wait after a connect
20 const Resetdelay \ how much to wait after a reset
20 const Enabledelay \ how much to wait after an enable
250 const Pollms \ port poll interval
100 const Chgdelay \ waiting for port become stable
