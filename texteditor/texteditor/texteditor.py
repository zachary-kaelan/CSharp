import sys
from tkinter import *
import tkinter.filedialog

root = Tk("Text Editor")

text = Text(root)
text.grid()

def saveas():
    global text
    saveLoc = tkinter.filedialog.asksaveasfilename()
    f = open(saveLoc, "w+")
    f.write(text.get("1.0","end-1c"))
    f.close()

btn = Button(root, text="Save", command=saveas)
btn.grid()

btnFont = Menubutton(root, text="Font")
btnFont.grid()
btnFont.menu=Menu(btnFont, tearoff=0)
btnFont["menu"] = btnFont.menu

root.mainloop()
