Component: Icons

https://icons8.com/

--------------------------------------------------

Component: PTMono Font
License: SIL Open Font License (OFL)

—————————————————————————————-
SIL OPEN FONT LICENSE Version 1.1 - 26 February 2007
—————————————————————————————-

PREAMBLE
The goals of the Open Font License (OFL) are to stimulate worldwide development of collaborative font projects, to support the font creation efforts of academic and linguistic communities, and to provide a free and open framework in which fonts may be shared and improved in partnership with others.

The OFL allows the licensed fonts to be used, studied, modified and redistributed freely as long as they are not sold by themselves. The fonts, including any derivative works, can be bundled, embedded, redistributed and/or sold with any software provided that any reserved names are not used by derivative works. The fonts and derivatives, however, cannot be released under any other type of license. The requirement for fonts to remain under this license does not apply to any document created using the fonts or their derivatives.

DEFINITIONS
“Font Software” refers to the set of files released by the Copyright Holder(s) under this license and clearly marked as such. This may include source files, build scripts and documentation.

“Reserved Font Name” refers to any names specified as such after the copyright statement(s).

“Original Version” refers to the collection of Font Software components as distributed by the Copyright Holder(s).

“Modified Version” refers to any derivative made by adding to, deleting, or substituting—in part or in whole—any of the components of the Original Version, by changing formats or by porting the Font Software to a new environment.

“Author” refers to any designer, engineer, programmer, technical writer or other person who contributed to the Font Software.

PERMISSION & CONDITIONS
Permission is hereby granted, free of charge, to any person obtaining a copy of the Font Software, to use, study, copy, merge, embed, modify, redistribute, and sell modified and unmodified copies of the Font Software, subject to the following conditions:

1) Neither the Font Software nor any of its individual components, in Original or Modified Versions, may be sold by itself.

2) Original or Modified Versions of the Font Software may be bundled, redistributed and/or sold with any software, provided that each copy contains the above copyright notice and this license. These can be included either as stand-alone text files, human-readable headers or in the appropriate machine-readable metadata fields within text or binary files as long as those fields can be easily viewed by the user.

3) No Modified Version of the Font Software may use the Reserved Font Name(s) unless explicit written permission is granted by the corresponding Copyright Holder. This restriction only applies to the primary font name as presented to the users.

4) The name(s) of the Copyright Holder(s) or the Author(s) of the Font Software shall not be used to promote, endorse or advertise any Modified Version, except to acknowledge the contribution(s) of the Copyright Holder(s) and the Author(s) or with their explicit written permission.

5) The Font Software, modified or unmodified, in part or in whole, must be distributed entirely under this license, and must not be distributed under any other license. The requirement for fonts to remain under this license does not apply to any document created using the Font Software.

TERMINATION
This license becomes null and void if any of the above conditions are not met.

DISCLAIMER
THE FONT SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO ANY WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT OF COPYRIGHT, PATENT, TRADEMARK, OR OTHER RIGHT. IN NO EVENT SHALL THE COPYRIGHT HOLDER BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, INCLUDING ANY GENERAL, SPECIAL, INDIRECT, INCIDENTAL, OR CONSEQUENTIAL DAMAGES, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF THE USE OR INABILITY TO USE THE FONT SOFTWARE OR FROM OTHER DEALINGS IN THE FONT SOFTWARE.

--------------------------------------------------

Component: Perfect DOS VGA 437 font
 /
/(_____________            ____
\              /______)\  |    |        
:\      |     /         \:|    |:::::::::: : .. . : ..  . .  :.    .
  \_____|    /      |    \|    |______  
___ /               ________          \...     .     .      .
\______________     \       |  |      /.. . .   .   .             .
               \            |__|     /
--x--x-----x----\______     |-/_____/-x--x-xx--x-- - -x -- - -   --  - - - 
. . . . . . . . . . . .\____|. . . . . .
-------------------------------------------------------------------------------
>> perfect dos vga 437 - general information  >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
-------------------------------------------------------------------------------

 "Perfect DOS VGA 437" and "Perfect DOS VGA 437 Win" are truetype fonts
 designed to emulate the MS-DOS/Text mode standard font, used on VGA monitors,
 with the 437 Codepage (standard US/International). This is a "bitmap" font,
 meaning it emulates a bitmap font and can only be used at a given size (8 or
 multiples of it like 16, 24, 32, etc). It's optimized for Flash too, so it
 won't produce antialias if used at round positions.
 
 There are two fonts available. "Perfect DOS VGA 437" uses the original DOS
 codepage 437. It should be used, for example, if you're opening DOS ASCII
 files on notepad or another windows-based editor. Since it's faithful to the
 original DOS codes, it won't accent correctly in windows ("é" would produce
 something different, not an "e" with an acute).
 
 There's also "Perfect DOS VGA 437 Win" which is the exactly same font adapted
 to a windows codepage. This should use accented characters correctly but won't
 work if you're opening a DOS-based text file.
 
 UPDATE: this is a new version, updated in august/2008. It has fixed leading
 metrics for Mac systems.

-------------------------------------------------------------------------------
>> perfect dos vga 437 - creation process >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
-------------------------------------------------------------------------------
 
 This font was created to be used on a Flash-based ANSi viewer I'm working. To
 create it, I created a small Quick Basic program to write all characters on
 screen,
 
  CLS
  FOR l = 0 TO 255
    charWrite 1 + (l MOD 20), 1 + (l \ 20) * 6 + (l MOD 2), LTRIM$(RTRIM$(STR$(l))) + CHR$(l)
  NEXT
  SUB charWrite (lin, col, char$)
    DEF SEG = &HB800
    FOR i = 1 TO LEN(char$)
      POKE ((lin - 1) * 160) + ((col - 2 + i) * 2), ASC(MID$(char$, i, 1))
      IF (i = LEN(char$)) THEN POKE ((lin - 1) * 160) + ((col - 2 + i) * 2) + 1, 113
    NEXT
  END SUB
  
 Then captured the text screen using SCREEN THIEF (a very, very old screen
 capture TSR program which converts text screens to images accurately). I then
 recreated the font polygon by polygon on Fontlab, while looking at the image
 on Photoshop. No conversion took place.

-------------------------------------------------------------------------------
>> copyright and stuff >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
-------------------------------------------------------------------------------

 This is a free font/file, distribute as you wish to who you wish. You are free
 to use it on a movie, a videogame, a video, a broadcast, without having to ask
 my permission.

 Please do not send me emails asking me to sign release forms if it require
 any sort of scanning or digital compositing. It's a big chore. This license
 file and a simple confirmation email should suffice as proof that you are
 allowed to use it.

 Of course I don't mind emails letting me know where something has been used.
 Those are always gratifying!

 Do NOT sell this font. It's not yours and you can't make money of it.

-------------------------------------------------------------------------------
>> author >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
-------------------------------------------------------------------------------

 Zeh Fernando
 zeh@zehfernando.com
 www.zehfernando.com

 rorshack ^ maiden brazil

-------------------------------------------------------------------------------
>> other notes >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
-------------------------------------------------------------------------------

 The year is now 2021. I would be remiss not to mention these more modern font
 packages:

 https://int10h.org/oldschool-pc-fonts/fontlist/

 They include VGA-like fonts and a bunch of other systems, easily supplanting
 the need for "Perfect DOS VGA" and then some.

 They use a Creative Commons license.

-------------------------------------------------------------------------------
^zehPULLSdahTRICK^kudosOUTtoWHOkeepsITreal^smashDAHfuckingENTAH!!!^lowres4ever^
-------------------------------------------------------------------------------

