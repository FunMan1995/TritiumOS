needs lib/fmt lib/wordtbl emul/6502 emul/virtio

create mem $10000 allot
mem $4000 "6502.img" 0 fdopen# nip fdread drop
mem new6502 const cpu
p"/data/cos.blk" bootfs open console ( storage out )
$78 const VIOOUT
mem VIOOUT + cpu ( storage out cmd cpu )
newvirtio const vio
vio enter
