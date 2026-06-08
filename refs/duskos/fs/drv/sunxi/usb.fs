needs lib/ival drv/timer drv/usb drv/usb/ehci
unit drv/sunxi/usb

\ CCU regs
$01c20000 absvalmap {
  +$060 uint BUS_CLK_GATING_REG0 ;
  +$0cc uint USBPHY_CFG_REG ;
  +$2c0 uint BUS_SOFT_RST_REG0 ;
}

$01c19400 absvalmap {
  +$10 uint PHY_CTL ;
  +$20 uint PHY_OTGCTL ;
}

$01c1a000 const OTGREGS
$01c1b000 const EHCIREGS
EHCIREGS value USBREGS
addrof USBREGS ivalmap {
  +$800 uint HCI_ICR ;
  +$804 uint HCI_STATUS ;
  +$810 N4 HCI_PHY_CTL ;
}

\ This logic here is a reconstitution of what u-boot does when booting the
\ Pine 64. The PHY_CTL port is *not* documented in the AllWinner user manual,
\ so I have no idea how this works. I'm just replicating the steps.
: ubootvodoo ( -- )
  doto HCI_PHY_CTL 2 invand |
  doto HCI_ICR $701 or |
  0 to PHY_OTGCTL ;

: sunxiusb$ ( -- )
  doto BUS_CLK_GATING_REG0 $33000000 or |
  doto BUS_SOFT_RST_REG0 $33000000 or |
  doto USBPHY_CFG_REG $30303 or |
  ubootvodoo
  USBREGS ehci$ 1 to CONFIGFLAG ;
