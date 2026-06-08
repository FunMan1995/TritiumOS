_start:
	nop
	mrs		x1, CurrentEL
	and     x1, x1, #0x0c
	cmp     x1, #0x0c	/* running at EL3? */
	bne     1f
	/* Yes, running at EL3, go to EL2 */
	mov	x1, sp
	msr	sp_el2, x1		/* Migrate SP */
	/* I don't know what that is, comes from https://gitlab.com/bztsrc/bootboot/-/blob/master/aarch64-rpi/boot.S */
    mov     x1, #0x5b1
    msr     scr_el3, x1
	mov     x1, #0x3c9	/* DEBUG|SERR|IRQ|FIQ|EL2H */
    msr     spsr_el3, x1
    adr     x1, 1f
    msr     elr_el3, x1
    eret
1:
	/* Alright, we're in EL2, now go to EL1 */
	/* ARM is such a shitshow */
	/* This bit comes from U-boot */
	/* Initialize Generic Timers */
	mrs	x0, cnthctl_el2
	/* Enable EL1 access to timers */
	orr	x0, x0, #0x03	/* CNTHCTL_EL2_EL1PCEN_EN|CNTHCTL_EL2_EL1PCTEN_EN */
	msr	cnthctl_el2, x0
	msr	cntvoff_el2, xzr

	/* Initialize MPID/MPIDR registers */
	mrs	x0, midr_el1
	msr	vpidr_el2, x0
	mrs	x0, mpidr_el1
	msr	vmpidr_el2, x0

	/* Disable coprocessor traps */
	mov	x0, #0x33ff			/* CPTR_EL2_RES1 */
	msr	cptr_el2, x0		/* Disable coprocessor traps to EL2 */
	msr	hstr_el2, xzr		/* Disable coprocessor traps to EL2 */
	mov	x0, #0x300000		/* CPACR_EL1_FPEN_EN */
	msr	cpacr_el1, x0		/* Enable FP/SIMD at EL1 */

	/* SCTLR_EL1 initialization
	 *
	 * setting RES1 bits (29,28,23,22,20,11) to 1
	 * and RES0 bits (31,30,27,21,17,13,10,6) +
	 * UCI,EE,EOE,WXN,nTWE,nTWI,UCT,DZE,I,UMA,SED,ITD,
	 * CP15BEN,SA0,SA,C,A,M to 0
	 */
    mov x0, #0x30d00000
	movk x0, #0x0800
	msr	sctlr_el1, x0

	mov	x0, sp
	msr	sp_el1, x0		/* Migrate SP */
	mrs	x0, vbar_el2
	msr	vbar_el1, x0		/* Migrate VBAR */

	/* Initialize HCR_EL2 */
	mov		x0, #0x20000000  /* AArch32, hypervisor disabled */
	msr	hcr_el2, x0

	mov		x0, #0x1d3		/* SERR|IRQ|FIQ|AARCH32|SVC */
	msr		spsr_el2, x0
    adr     x0, 1f
	msr		elr_el2, x0
	eret

.org 0x100
1:
