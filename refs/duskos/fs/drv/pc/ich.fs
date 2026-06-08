\ Intel Controller Hub chipset family
\ This unit implement extra PCI fields that are specific to this chipset
needs drv/pc/pci
unit drv/pc/ich

1 const ICHFIELDCNT
create ichfieldlist ICHFIELDCNT 4* allot
ichfieldlist to _currentlist

\ This is where you switch between IDE/AHCI/RAID modes of the SATA controller.
$90 pcifield pciich.map "MAP"

: .pciich ( addr -- ) dup pciexists# ichfieldlist ICHFIELDCNT .pcifields ;
