/* Source: http://www.9legacy.org/ 4th release from 2015-01-10
 * Filenames: sys/src/9/bcm/dwcotg.h
              sys/src/9/bcm/usbdwc.c
              sys/src/9/bcm/devusb.c
 * License: /licenses/plan9legacy
 * Extra copyright in usbdwc.c:
 * Copyright © 2012 Richard Miller <r.miller@acm.org>
 */

#define BLEN ((%<)->wp - (%0)->rp)
// HOWMANY "x" "y" --> how many of x fit in y?
#define HOWMANY (((%<)+((%<)-1))/(%1))
// ROUND "s" "sz" --> round s to the nearest sz
#define ROUND (((%<)+(%<-1))&~(%1-1))

enum {
  /* gahbcfg */
  Glblintrmsk = 1<<0,
  /* bits 1:4 redefined for BCM2835 */
  Axiburstlen = 0x3<<1,
    BURST1      = 3<<1,
    BURST2      = 2<<1,
    BURST3      = 1<<1,
    BURST4      = 0<<1,
  Axiwaitwrites    = 1<<4,
  Dmaenable        = 1<<5,
  Nptxfemplvl      = 1<<7,
    NPTX_HALFEMPTY = 0<<7,
    NPTX_EMPTY     = 1<<7,
  Ptxfemplvl       = 1<<8,
    PTX_HALFEMPTY  = 0<<8,
    PTX_EMPTY      = 1<<8,
  Remmemsupp       = 1<<21,
  Notialldmawrit   = 1<<22,
  Ahbsingle        = 1<<23,

  /* gusbcfg */
  Toutcal        = 0x7<<0,
  Phyif          = 1<<3,
  Ulpi_utmi_sel  = 1<<4,
  Fsintf         = 1<<5,
    FsUnidir     = 0<<5,
    FsBidir      = 1<<5,
  Physel         = 1<<6,
    PhyHighspeed = 0<<6,
    PhyFullspeed = 1<<6,
  Ddrsel         = 1<<7,
  Srpcap         = 1<<8,
  Hnpcap         = 1<<9,
  Usbtrdtim      = 0xf<<10,
    OUsbtrdtim   = 10,
  Phylpwrclksel  = 1<<15,
  Otgutmifssel   = 1<<16,
  Ulpi_fsls      = 1<<17,
  Ulpi_auto_res  = 1<<18,
  Ulpi_clk_sus_m = 1<<19,
  Ulpi_ext_vbus_drv = 1<<20,
  Ulpi_int_vbus_indicator = 1<<21,
  Term_sel_dl_pulse = 1<<22,
  Indicator_complement = 1<<23,
  Indicator_pass_through = 1<<24,
  Ulpi_int_prot_dis = 1<<25,
  Ic_usb_cap = 1<<26,
  Ic_traffic_pull_remove = 1<<27,
  Tx_end_delay    = 1<<28,
  Force_host_mode = 1<<29,
  Force_dev_mode  = 1<<30,

  /* grstctl */
  Csftrst     = 1<<0,
  Hsftrst     = 1<<1,
  Hstfrm      = 1<<2,
  Intknqflsh  = 1<<3,
  Rxfflsh     = 1<<4,
  Txfflsh     = 1<<5,
  Txfnum      = 0x1f<<6,
    TXF_ALL   = 0x10<<6,
  Dmareq      = 1<<30,
  Ahbidle     = 1<<31,

  /* gintsts, gintmsk */
  Curmode       = 1<<0,
    HOSTMODE    = 1<<0,
    DEVMODE     = 0<<0,
  Modemismatch  = 1<<1,
  Otgintr     = 1<<2,
  Sofintr     = 1<<3,
  Rxstsqlvl   = 1<<4,
  Nptxfempty  = 1<<5,
  Ginnakeff   = 1<<6,
  Goutnakeff  = 1<<7,
  Ulpickint   = 1<<8,
  I2cintr     = 1<<9,
  Erlysuspend = 1<<10,
  Usbsuspend  = 1<<11,
  Usbreset    = 1<<12,
  Enumdone    = 1<<13,
  Isooutdrop  = 1<<14,
  Eopframe    = 1<<15,
  Restoredone = 1<<16,
  Epmismatch  = 1<<17,
  Inepintr    = 1<<18,
  Outepintr   = 1<<19,
  Incomplisoin  = 1<<20,
  Incomplisoout = 1<<21,
  Fetsusp     = 1<<22,
  Resetdet    = 1<<23,
  Portintr    = 1<<24,
  Hcintr      = 1<<25,
  Ptxfempty   = 1<<26,
  Lpmtranrcvd = 1<<27,
  Conidstschng = 1<<28,
  Disconnect  = 1<<29,
  Sessreqintr = 1<<30,
  Wkupintr    = 1<<31,

  /* hptxfsiz, gnptxfsiz */
  Startaddr   = 0xffff<<0,
  Depth       = 0xffff<<16,
    ODepth    = 16,

  /* ghwcfg2 */
  Op_mode = 0x7<<0,
    HNP_SRP_CAPABLE_OTG = 0<<0,
    SRP_ONLY_CAPABLE_OTG = 1<<0,
    NO_HNP_SRP_CAPABLE  = 2<<0,
    SRP_CAPABLE_DEVICE  = 3<<0,
    NO_SRP_CAPABLE_DEVICE = 4<<0,
    SRP_CAPABLE_HOST    = 5<<0,
    NO_SRP_CAPABLE_HOST = 6<<0,
  Architecture    = 0x3<<3,
      SLAVE_ONLY  = 0<<3,
      EXT_DMA     = 1<<3,
      INT_DMA     = 2<<3,
  Point2point = 1<<5,
  Hs_phy_type = 0x3<<6,
    PHY_NOT_SUPPORTED = 0<<6,
    PHY_UTMI        = 1<<6,
    PHY_ULPI        = 2<<6,
    PHY_UTMI_ULPI   = 3<<6,
  Fs_phy_type = 0x3<<8,
  Num_dev_ep  = 0xf<<10,
  Num_host_chan    = 0xf<<14,
    ONum_host_chan = 14,
  Perio_ep_supported = 1<<18,
  Dynamic_fifo = 1<<19,
  Nonperio_tx_q_depth= 0x3<<22,
  Host_perio_tx_q_depth= 0x3<<24,
  Dev_token_q_depth= 0x1f<<26,
  Otg_enable_ic_usb= 1<<31,

  /* ghwcfg3 */
  Xfer_size_cntr_width    = 0xf<<0,
  Packet_size_cntr_width  = 0x7<<4,
  Otg_func    = 1<<7,
  I2c         = 1<<8,
  Vendor_ctrl_if      = 1<<9,
  Optional_features   = 1<<10,
  Synch_reset_type    = 1<<11,
  Adp_supp        = 1<<12,
  Otg_enable_hsic = 1<<13,
  Bc_support      = 1<<14,
  Otg_lpm_en      = 1<<15,
  Dfifo_depth     = 0xffff<<16,
    ODfifo_depth  = 16,

  /* hfnum */
  Frnum       = 0xffff<<0,
    MAX_FRNUM   = 0x3FFF<<0,
  Frrem       = 0xffff<<16,

  /* hport0 */
  Prtconnsts  = 1<<0,     /* connect status (RO) */
  Prtconndet  = 1<<1,     /* connect detected R/W1C) */
  Prtena      = 1<<2,     /* enable (R/W1C) */
  Prtenchng   = 1<<3,     /* enable/disable change (R/W1C) */
  Prtovrcurract  = 1<<4,     /* overcurrent active (RO) */
  Prtovrcurrchng = 1<<5,     /* overcurrent change (R/W1C) */
  Prtres      = 1<<6,     /* resume */
  Prtsusp     = 1<<7,     /* suspend */
  Prtrst      = 1<<8,     /* reset */
  Prtlnsts    = 0x3<<10,  /* line state {D+,D-} (RO) */
  Prtpwr      = 1<<12,    /* power on */
  Prttstctl   = 0xf<<13,  /* test */
  Prtspd      = 0x3<<17,  /* speed (RO) */
    HIGHSPEED = 0<<17,
    FULLSPEED = 1<<17,
    LOWSPEED  = 2<<17,

  /* hcchar */
  Mps      = 0x7ff<<0, /* endpoint maximum packet size */
  Epnum    = 0xf<<11,  /* endpoint number */
    OEpnum = 11,
  Epdir    = 1<<15,    /* endpoint direction */
    Epout  = 0<<15,
    Epin   = 1<<15,
  Lspddev  = 1<<17,    /* device is lowspeed */
  Eptype   = 0x3<<18,  /* endpoint type */
    Epctl  = 0<<18,
    Episo  = 1<<18,
    Epbulk = 2<<18,
    Epintr = 3<<18,
  Multicnt = 0x3<<20,  /* transactions per μframe */
                       /* or retries per periodic split */
    OMulticnt = 20,
  Devaddr     = 0x7f<<22, /* device address */
    ODevaddr  = 22,
  Oddfrm      = 1<<29,    /* xfer in odd frame (iso/interrupt) */
  Chdis       = 1<<30,    /* channel disable (write 1 only) */
  Chen        = 1<<31,    /* channel enable (write 1 only) */

  /* hcsplt */
  Prtaddr     = 0x7f<<0,  /* port address of recipient */
                          /* transaction translator */
  Hubaddr     = 0x7f<<7,  /* dev address of transaction */
                          /* translator's hub */
    OHubaddr  = 7,
  Xactpos     = 0x3<<14,  /* payload's position within transaction */
    POS_MID   = 0<<14,
    POS_END   = 1<<14,
    POS_BEGIN = 2<<14,
    POS_ALL   = 3<<14,    /* all of data (<= 188 bytes) */
  Compsplt    = 1<<16,    /* do complete split */
  Spltena     = 1<<31,    /* channel enabled to do splits */

  /* hcint, hcintmsk */
  Xfercomp    = 1<<0,     /* transfer completed without error */
  Chhltd      = 1<<1,     /* channel halted */
  Ahberr      = 1<<2,     /* AHB dma error */
  Stall       = 1<<3,
  Nak         = 1<<4,
  Ack         = 1<<5,
  Nyet        = 1<<6,
  Xacterr     = 1<<7, /* transaction error (crc, t/o, bit stuff, eop) */
  Bblerr      = 1<<8,
  Frmovrun    = 1<<9,
  Datatglerr  = 1<<10,
  Bna         = 1<<11,
  Xcs_xact    = 1<<12,
  Frm_list_roll = 1<<13,

  /* hctsiz */
  Xfersize    = 0x7ffff<<0,   /* expected total bytes */
  Pktcnt      = 0x3ff<<19,    /* expected number of packets */
    OPktcnt   = 19,
  Pid     = 0x3<<29,  /* packet id for initial transaction */
    DATA0       = 0<<29,
    DATA1       = 2<<29,    /* sic */
    DATA2       = 1<<29,    /* sic */
    MDATA       = 3<<29,    /* (non-ctl ep) */
    SETUP       = 3<<29,    /* (ctl ep) */
  Dopng         = 1<<31,    /* do PING protocol */

  Enabledelay = 50,
  Resetdelay  = 10,
  ResetdelayHS = 50,
};

void abortmsg(uchar* msg) { stype(msg); abort(); }
void assert(uint n) { if (!n) abortmsg("assertion error"); }

// sys/src/9/bcm/usbdwc.c
Hostchan* chanalloc(DWCEp *ep) {
  DWCHci *ctlr;
  int bitmap, i;

  ctlr = (DWCHci*)ep->hci;
  return &ctlr->regs->hchan[0];
}

void chansetup(Hostchan *hc, DWCEp *ep) {
  uint hcc;
  DWCHci *ctlr = (DWCHci*)ep->hci;

  switch (ep->dev->state) {
  case Dconfig:
  case Dreset:
    hcc = 0;
    break;
  default:
    hcc = ep->dev->nb<<ODevaddr;
    break;
  }
  hcc |= ep->maxpkt | 1<<OMulticnt | ep->nb<<OEpnum;
  switch (ep->ttype) {
  case Tctl:
    hcc |= Epctl;
    break;
  case Tiso:
    hcc |= Episo;
    break;
  case Tbulk:
    hcc |= Epbulk;
    break;
  case Tintr:
    hcc |= Epintr;
    break;
  }
  switch (ep->dev->speed) {
  case Lowspeed:
    hcc |= Lspddev;
    /* fall through */
  case Fullspeed:
    if (ep->dev->parenthub->hubdev) {
      hc->hcsplt =
        Spltena | POS_ALL | ep->dev->parenthub->hubdev->nb<<OHubaddr | ep->dev->portnb;
      break;
    }
    /* fall through */
  default:
    hc->hcsplt = 0;
    break;
  }
  hc->hcchar = (uint)hcc;
  hc->hcint = ~0;
}

uint sofdone(void *a) {
  Dwcregs *r;

  r = a;
  return r->gintsts & Sofintr;
}

void sofwait(DWCHci *ctlr, int n) {
  Dwcregs *r;
  int x;

  r = ctlr->regs;
  do {
    waitus(10);
  } while((r->hfnum & 7) == 6);
}

uint chanwait(DWCEp *ep, DWCHci *ctlr, Hostchan *hc, uint mask) {
  uint intr, ointr, start, now, n;
  Dwcregs *r;

  r = ctlr->regs;
  n = (uint)(hc - r->hchan);
  while (1) {
restart:
    r->haintmsk |= 1<<n;
    hc->hcintmsk = mask;
    waitus(100);
    intr = hc->hcint;
    if (!(intr & hc->hcintmsk) && intr != (Chhltd|Ack)) goto restart;
    hc->hcintmsk = 0;
    if (intr & Chhltd) return intr;
    start = ticks();
    ointr = intr;
    now = start;
    do {
      intr = hc->hcint;
      if (intr & Chhltd) {
        return intr;
      }
      if ((intr & mask) == 0) {
        goto restart;
      }
      now = ticks();
    } while(now - start < 100);
    mask = Chhltd;
    hc->hcchar |= Chdis;
    start = ticks();
    while (hc->hcchar & Chen) {
      if (ticks() - start >= 100) {
        printf("ep%d.%d channel won't halt hcchar %x\n",
          ep->dev->nb, ep->nb, hc->hcchar);
        break;
      }
    }
  }
  abortmsg("unreachable!");
}

// TODO: straighten this convoluted execution model. It's the result of a
// straight porting from plan9, which uses interrupt, but we can do much better
// than this...
uint chanintr(DWCHci *ctlr, Hostchan *hc) {
  uint i;

  i = hc->hcint;
  if (i == (Chhltd|Ack)) {
    hc->hcsplt |= Compsplt;
    ctlr->splitretry = 0;
  } else if (i == (Chhltd|Nyet)) {
    if (++ctlr->splitretry >= 3) return 0;
  } else return 0;
  if (hc->hcchar & Chen) {
    printf("hcchar %x hcint %x", hc->hcchar, hc->hcint);
    hc->hcchar |= Chen | Chdis;
    while (hc->hcchar&Chen);
    printf(" %x\n", hc->hcint);
  }
  hc->hcint = i;
  if (ctlr->regs->hfnum & 1) hc->hcchar &= ~Oddfrm;
  else hc->hcchar |= Oddfrm;
  hc->hcchar = (hc->hcchar &~ Chdis) | Chen;
  return 1;
}

uint chanio(DWCEp *ep, Hostchan *hc, uint dir, uint pid, void *a, uint len) {
  DWCHci *ctlr;
  uint hcdma, hctsiz, maxpkt, npkt, n, nleft, nt, i;

  ctlr = (DWCHci*)ep->hci;
  maxpkt = ep->maxpkt;
  npkt = HOWMANY "len" "maxpkt";
  if (npkt == 0) npkt = 1;

  hc->hcchar = (hc->hcchar & ~Epdir) | dir;
  if (dir == Epin) n = ROUND "len" "maxpkt";
  else n = len;
  hc->hctsiz = (uint)n | npkt<<OPktcnt | pid;
  hc->hcdma = (uint)a | DMABASE;

  nleft = len;
  while (1) {
    hcdma = hc->hcdma;
    hctsiz = hc->hctsiz;
    hc->hctsiz = hctsiz & ~Dopng;
    if (hc->hcchar&Chen) {
      printf("ep%d.%d before chanio hcchar=%x\n",
        ep->dev->nb, ep->nb, hc->hcchar);
      hc->hcchar |= Chen | Chdis;
      while (hc->hcchar&Chen);
      hc->hcint = Chhltd;
    }
    if ((i = hc->hcint) != 0) {
      printf("ep%d.%d before chanio hcint=%x\n", ep->dev->nb, ep->nb, i);
      hc->hcint = i;
    }
    if (hc->hcsplt & Spltena) {
      sofwait(ctlr, hc - ctlr->regs->hchan);
      if((ctlr->regs->hfnum & 1) == 0) hc->hcchar &= ~Oddfrm;
      else hc->hcchar |= Oddfrm;
    }
    hc->hcchar = (hc->hcchar &~ Chdis) | Chen;
    do {
      if (ep->ttype == Tbulk && dir == Epin) {
        i = chanwait(ep, ctlr, hc, /* Ack| */ Chhltd);
      } else if (ep->ttype == Tintr && (hc->hcsplt & Spltena)) {
        i = chanwait(ep, ctlr, hc, Chhltd);
      } else {
        i = chanwait(ep, ctlr, hc, Chhltd|Nak);
      }
    } while ((hc->hcsplt & Spltena) && chanintr(ctlr, hc));

    hc->hcint = (uint)i;

    if (hc->hcsplt & Spltena) {
      hc->hcsplt &= ~Compsplt;
    }

    if ((i & Xfercomp) == 0 && i != (Chhltd|Ack) && i != Chhltd) {
      if (i & Stall) { return 0; };
      if (i & (Nyet|Frmovrun)) continue;
      if (i & Nak) {
        waitms(2);
        // TODO: adapt usbcmd to handle epread() returning 0 on NAK
        if ((ep->ttype != Tctl) && (dir == Epin)) return 0;
        continue;
      }
      printf("usbotg: ep%d.%d error intr %x\n", ep->dev->nb, ep->nb, i);
      if(i & ~(Chhltd|Ack)) { stype("Eio\n"); return 0; }
      if(hc->hcdma != hcdma) {
        printf("usbotg: weird hcdma %x->%x intr %x->%x\n",
          hcdma, hc->hcdma, i, hc->hcint);
      }
    }
    n = (hc->hcdma - hcdma);
    if (n == 0) {
      if ((hc->hctsiz & Pktcnt) != (hctsiz & Pktcnt)) break;
      else if (!len) {
        // Under QEMU, Pktcnt doesn't decrease when Xfersize is zero, but on a
        // read DWC, it does. This shouldn't happen unless in QEMU.
        break;
      } else continue;
    }
    if (dir == Epin && ep->ttype == Tbulk && n == nleft) {
      nt = ((hctsiz & Xfersize) - (hc->hctsiz & Xfersize));
      if (nt != n) {
        if(n == ROUND "nt" "4") n = nt;
        else {
          printf("usbotg: intr %x dma %x-%x hctsiz %x-%x\n",
            i, hcdma, hc->hcdma, hctsiz, hc->hctsiz);
        }
      }
    }
    if (n > nleft) {
      if (n != ROUND "nleft" "4") {
        printf("too much: wanted %d got %d\n", len, len - nleft + n);
      }
      n = nleft;
    }
    nleft -= n;
    if (nleft == 0 || (n % maxpkt) != 0) break;
    if ((i & Xfercomp) && ep->ttype != Tctl) break;
    if (dir == Epout) {
      printf("too little: nleft %d hcdma %x->%x hctsiz %x->%x intr %x\n",
        nleft, hcdma, hc->hcdma, hctsiz, hc->hctsiz, i);
    }
  }
  return len - nleft;
}

uint multitrans(DWCEp *ep, Hostchan *hc, uint dir, void *a, uint n) {
  uint sofar;
  uint m;

  sofar = 0;
  do {
    m = n - sofar;
    if (m > ep->maxpkt) m = ep->maxpkt;
    m = chanio(ep, hc, dir, ep->toggle, (char*)a + sofar, m);
    ep->toggle = hc->hctsiz & Pid;
    sofar += m;
  } while (sofar < n && m == ep->maxpkt);
  return sofar;
}

uint eptrans(DWCEp *ep, uint dir, void *a, uint n) {
  Hostchan *hc;

  if (ep->clrhalt) {
    ep->clrhalt = 0;
    ep->toggle = DATA0;
  }
  hc = chanalloc(ep);
  chansetup(hc, ep);
  if (dir == Epin && ep->ttype == Tbulk) {
    n = multitrans(ep, hc, dir, a, n);
  } else {
    n = chanio( ep, hc, dir, ep->toggle, a, n);
    ep->toggle = hc->hctsiz & Pid;
  }
  return n;
}

uint ctltrans(DWCEp *ep, uchar *req, uint n) {
  Hostchan *hc;
  Block *b;
  uchar *data;
  uint datalen, datasz;

  ep->epbuf = NULL;
  if(n < Rsetuplen) abortmsg("Ebadlen");
  if(req[Rtype] & Rd2h){
    datalen = (uint)`wle@ (req+Rcount);
    if (datalen <= 0 || datalen > Maxctllen) abortmsg("Ebadlen");
    datasz = ROUND "datalen" "ep->maxpkt";
    data = usbpadallot(datasz);
    b = allocb(data, datasz);
    ep->epbuf = b;
    if (datasz) cfill(0x55, datasz, data);
  } else {
    b = NULL;
    datalen = n - Rsetuplen;
    data = req + Rsetuplen;
  }
  hc = chanalloc(ep);
  chansetup(hc, ep);
  chanio(ep, hc, Epout, SETUP, req, Rsetuplen);
  if (req[Rtype] & Rd2h) {
    if (!ep->dev->parenthub->hubdev) {
      ep->toggle = DATA1;
      b->wp += (int)multitrans(ep, hc, Epin, data, datalen);
    } else {
      b->wp += (int)chanio(ep, hc, Epin, DATA1, data, datalen);
    }
    chanio(ep, hc, Epout, DATA1, NULL, 0);
    n = Rsetuplen;
  } else {
    if(datalen > 0) {
      chanio(ep, hc, Epout, DATA1, data, datalen);
    }
    chanio(ep, hc, Epin, DATA1, NULL, 0);
    n = Rsetuplen + datalen;
  }
  return n;
}

uint ctldata(DWCEp *ep, void *a, uint n) {
  Block *b;

  b = ep->epbuf;
  if (b == NULL) return 0;
  if(n > BLEN "b") n = BLEN "b";
  cmove(n, a, b->rp);
  b->rp += n;
  if (BLEN "b" == 0) {
    ep->epbuf = NULL;
  }
  return n;
}

void greset(Dwcregs *r, uint bits) {
  r->grstctl |= bits;
  while(r->grstctl & bits);
  waitus(10);
}

void init(DWCHci *ctlr) {
  Dwcregs *r;
  uint n, rx, tx, ptx;

  r = ctlr->regs;

  ctlr->nchan = 1 + ((r->ghwcfg2 & Num_host_chan) >> ONum_host_chan);

  r->gahbcfg = 0;
  setpower(PowerUsb, 1);

  while((r->grstctl&Ahbidle) == 0) waitus(1);
  greset(r, Csftrst);

  r->gusbcfg |= Force_host_mode;
  waitms(25);
  r->gahbcfg |= Dmaenable;

  n = (r->ghwcfg3 & Dfifo_depth) >> ODfifo_depth;
  rx = 0x306;
  tx = 0x100;
  ptx = 0x200;
  r->grxfsiz = rx;
  r->gnptxfsiz = rx | tx<<ODepth;
  waitms(1);
  r->hptxfsiz = (rx + tx) | ptx << ODepth;
  greset(r, Rxfflsh);
  r->grstctl = TXF_ALL;
  greset(r, Txfflsh);
  r->hport0 = Prtpwr|Prtconndet|Prtenchng|Prtovrcurrchng;
  r->gintsts = ~0;
  r->gintmsk = Hcintr;
}

uint _epread(DWCEp *ep, void *a, uint n) {
  switch(ep->ttype){
  case Tctl:
    return ctldata(ep, a, n);
  case Tintr:
  case Tbulk:
    return eptrans(ep, Epin, a, n);
  default:
    abortmsg("Egreg");
  }
}

uint _epwrite(DWCEp *ep, void *a, uint n) {
  uchar *p;

  switch (ep->ttype) {
  case Tintr:
    /* fall through */
  case Tctl:
  case Tbulk:
    p = usbpadallot(n); // "a" might not be properly aligned
    cmove(n, p, a);
    if(ep->ttype == Tctl) {
      n = ctltrans(ep, p, n);
    } else {
      n = eptrans(ep, Epout, p, n);
    }
    return n;
  default:
    abortmsg("Egreg");
  }
}

uint portenable(DWCHci *ctlr, uint port, uint on) {
  Dwcregs *r;

  assert(port == 1);
  r = ctlr->regs;
  if (!on) r->hport0 = Prtpwr | Prtena;
  waitms(Enabledelay);
  return 0;
}

uint portreset(DWCHci *ctlr, uint port, uint on) {
  Dwcregs *r;
  uint b, s;

  assert(port == 1);
  r = ctlr->regs;
  if (!on) return 0;
  r->hport0 = Prtpwr | Prtrst;
  waitms(ResetdelayHS);
  r->hport0 = Prtpwr;
  waitms(Enabledelay);
  s = r->hport0;
  b = s & (Prtconndet|Prtenchng|Prtovrcurrchng);
  if (b != 0) r->hport0 = Prtpwr | b;
  if ((s & Prtena) == 0) {
    stype("usbotg: host port not enabled after reset");
  }
  return 0;
}

uint portstatus(DWCHci *ctlr, uint port) {
  Dwcregs *r;
  uint b, s;

  assert(port == 1);
  r = ctlr->regs;
  s = r->hport0;
  b = s & (Prtconndet|Prtenchng|Prtovrcurrchng);
  if (b != 0) r->hport0 = Prtpwr | b;
  b = 0;
  if (s & Prtconnsts) b |= PSpresent;
  if (s & Prtconndet) b |= PSstatuschg;
  if (s & Prtena) b |= PSenable;
  if (s & Prtenchng) b |= PSchange;
  if (s & Prtovrcurract) b |= PSovercurrent;
  if (s & Prtsusp) b |= PSsuspend;
  if (s & Prtrst) b |= PSreset;
  if (s & Prtpwr) b |= PSpower;
  switch (s & Prtspd) {
  case HIGHSPEED:
    b |= PShigh;
    break;
  case LOWSPEED:
    b |= PSslow;
    break;
  }
  return b;
}

uint _roothubfeature(DWCHci *ctlr, uint port, uint feature, uint on) {
  if (port < 1 || port > ctlr->nports) abortmsg("bad hub port number");
  switch (feature) {
  case Fportenable: return portenable(ctlr, port, on);
  case Fportreset: return portreset(ctlr, port, on);
  case Rgetstatus: return portstatus(ctlr, port);
  default: return 0;
  }
}
