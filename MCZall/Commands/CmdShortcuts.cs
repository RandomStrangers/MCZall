//TODO: Read from Txt
/*
namespace MCZall.Commands
{

    public class CmdShortcuts : Command
    {
        public override string Name { get { return "shortcuts"; } }
        public CmdShortcuts() { }
        public override string Type { get { return "information"; } }
        public override string Shortcut { get { return "sc"; } }
        public override void Use(Player p, string message)
        {
            if (message != "") { Help(p); return; }
            message += "a(abort), ";
            message += "b(about), ";
            message += "c(copy), ";
            message += "e(spheroid), ";
            message += "f(fill), ";
            message += "g(goto), ";
            message += "k(kick), ";
            message += "l(lava), ";
            message += "o(portal), ";
            message += "p(paint), ";
            message += "r(replace), ";
            message += "s(summon), ";
            message += "u(undo), ";
            message += "v(paste), ";
            message += "w(water), ";
            message += "x(cut), ";
            message += "z(cuboid), ";
            message += "bi(banip), ";
            message += "kb(kickban), ";
            message += "rn(replacenot), ";
            message += "rp(restartphysics), ";
            message += "zm(megaboid), ";
            message += "rank(setrank)";

            p.SendMessage(message);
        }

        public override void Help(Player p)
        {
            p.SendMessage("/shortcuts - Lists every shortcut available");
        }
    }
}
*/