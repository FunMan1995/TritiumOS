\ This unit has the signature ( reservedsectors secperclus imgpath -- )
create imgpath s,
const spc
const rsvd

needs fs/fatt
imgpath mountImage value mydrv
fatopts$
spc to secperclus
rsvd to rsvdsec
mydrv newFAT
bye
