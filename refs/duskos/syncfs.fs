\ expects an image name in PS top with an already created FAT in it
create imgpath s,

needs fs/sh fs/fat

imgpath mountImage value mydrv
walkdst mydrv newfatfs walk
walksrc p"" copyall
bye
