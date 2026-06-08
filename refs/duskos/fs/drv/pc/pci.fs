needs drv/pci drv/pc/ioport
unit drv/pc/pci

$cf8 ioport cfgaddr
$cfc ioport cfgdata
: pci@ ( addr -- n ) to cfgaddr cfgdata ;
: pci! ( n addr -- n ) to cfgaddr to cfgdata ;
: pcpci$ ['] pci@ ['] pci! pci$ ;
