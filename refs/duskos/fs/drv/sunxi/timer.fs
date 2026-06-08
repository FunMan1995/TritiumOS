needs lib/ival drv/timer
unit drv/sunxi/timer

$01c20c00 absvalmap {
  +$00 uint TMR_IRQ_EN_REG TMR_IRQ_STA_REG ;
  +$10 uint TMR0_CTRL_REG TMR0_INTV_VALUE_REG TMR0_CUR_VALUE_REG ;
  +$20 uint TMR1_CTRL_REG TMR1_INTV_VALUE_REG TMR1_CUR_VALUE_REG ;
  +$80 uint AVS_CNT_CTL_REG AVS_CNT0_REG AVS_CNT1_REG AVS_CNT_DIV_REG ;
  +$a0 uint WDOG0_IRQ_EN_REG WDOG0_IRQ_STA_REG ;
  +$b0 uint WDOG0_CTRL_REG WDOG0_CFG_REG WDOG0_MODE_REG ;
}

: (ticks) TMR0_CUR_VALUE_REG neg ;
: sunxitimer$
  \ Have the timer 0 run on the 32kHz clock in continuous mode
  -1 to TMR0_INTV_VALUE_REG 3 to TMR0_CTRL_REG
  ['] (ticks) ['] noop 1000 32 / timer$ ;
