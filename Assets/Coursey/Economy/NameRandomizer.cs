using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Economy
{
    public static class NameRandomizer
    {
        private const bool _debugThisClass = true;
        private static readonly List<string> vowelPairs = new();
        private static readonly List<string> vowels = new List<string> 
        {
            "a", "e", "i", "o", "u"
        };
        private static readonly List<string> nonVowels = new List<string>
        { 
            "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "z"
        };
        private static readonly List<string> alphabet = new List<string>
        {
            "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"
        };
        private static List<string> letterPairList;

        private readonly static Dictionary<string, int> letterPairWeights = new Dictionary<string, int>();
        private static readonly Dictionary<int, int> letterWeights = new Dictionary<int, int>();//in 10k
        private static Dictionary<NameType, List<string>> namesGenerated = new Dictionary<NameType, List<string>>();
        private static int totalLetterChance = 0;
        private static void GenerateVowelPairs()
        {
            for(int i = 0; i < vowels.Count; i++)
            {
                for(int j = 0; j < vowels.Count; j++)
                {
                    string vowelPairToAdd = vowels[i] + vowels[j];
                    if (!vowelPairs.Contains(vowelPairToAdd))
                    {
                        vowelPairs.Add(vowelPairToAdd);
                    }
                }
            }
        }
        public static void Initialize()
        {
            foreach(NameType nameType in (NameType[])Enum.GetValues(typeof(NameType)))
            {
                namesGenerated.Add(nameType, new List<string>());
            }
            GenerateVowelPairs();
            EstablishLetterWeights();
            letterPairList = letterPairWeights.Keys.ToList();
        }
        private static void EstablishLetterWeights()
        {//changed ox, hh, qi, dd
            //chrome-extension://efaidnbmnnnibpcajpcglclefindmkaj/http://homepages.math.uic.edu/~leon/mcs425-s08/handouts/char_freq2.pdf
            letterPairWeights["aa"] = 1; letterPairWeights["ab"] = 20; letterPairWeights["ac"] = 33; letterPairWeights["ad"] = 52; letterPairWeights["ae"] = 0; letterPairWeights["af"] = 12; letterPairWeights["ag"] = 18; letterPairWeights["ah"] = 5; letterPairWeights["ai"] = 39; letterPairWeights["aj"] = 1; letterPairWeights["ak"] = 12; letterPairWeights["al"] = 57; letterPairWeights["am"] = 26; letterPairWeights["an"] = 181; letterPairWeights["ao"] = 1; letterPairWeights["ap"] = 20; letterPairWeights["aq"] = 1; letterPairWeights["ar"] = 75; letterPairWeights["as"] = 95; letterPairWeights["at"] = 104; letterPairWeights["au"] = 9; letterPairWeights["av"] = 20; letterPairWeights["aw"] = 13; letterPairWeights["ax"] = 1; letterPairWeights["ay"] = 26; letterPairWeights["az"] = 1;
            letterPairWeights["ba"] = 11; letterPairWeights["bb"] = 1; letterPairWeights["bc"] = 0; letterPairWeights["bd"] = 0; letterPairWeights["be"] = 47; letterPairWeights["bf"] = 0; letterPairWeights["bg"] = 0; letterPairWeights["bh"] = 0; letterPairWeights["bi"] = 6; letterPairWeights["bj"] = 1; letterPairWeights["bk"] = 0; letterPairWeights["bl"] = 17; letterPairWeights["bm"] = 0; letterPairWeights["bn"] = 0; letterPairWeights["bo"] = 19; letterPairWeights["bp"] = 0; letterPairWeights["bq"] = 0; letterPairWeights["br"] = 11; letterPairWeights["bs"] = 2; letterPairWeights["bt"] = 1; letterPairWeights["bu"] = 21; letterPairWeights["bv"] = 0; letterPairWeights["bw"] = 0; letterPairWeights["bx"] = 0; letterPairWeights["by"] = 11; letterPairWeights["bz"] = 0;
            letterPairWeights["ca"] = 31; letterPairWeights["cc"] = 4; letterPairWeights["cd"] = 0; letterPairWeights["ce"] = 38; letterPairWeights["cf"] = 0; letterPairWeights["cg"] = 0; letterPairWeights["cb"] = 0; letterPairWeights["ch"] = 38; letterPairWeights["ci"] = 10; letterPairWeights["cj"] = 0; letterPairWeights["ck"] = 18; letterPairWeights["cl"] = 9; letterPairWeights["cm"] = 0; letterPairWeights["cn"] = 0; letterPairWeights["co"] = 45; letterPairWeights["cp"] = 0; letterPairWeights["cq"] = 1; letterPairWeights["cr"] = 11; letterPairWeights["cs"] = 1; letterPairWeights["ct"] = 15; letterPairWeights["cu"] = 7; letterPairWeights["cv"] = 0; letterPairWeights["cw"] = 0; letterPairWeights["cx"] = 0; letterPairWeights["cy"] = 1; letterPairWeights["cz"] = 0;
            letterPairWeights["da"] = 48; letterPairWeights["db"] = 20; letterPairWeights["dc"] = 9; letterPairWeights["dd"] = 6; letterPairWeights["de"] = 57; letterPairWeights["df"] = 11; letterPairWeights["dg"] = 7; letterPairWeights["dh"] = 25; letterPairWeights["di"] = 50; letterPairWeights["dj"] = 3; letterPairWeights["dk"] = 1; letterPairWeights["dl"] = 11; letterPairWeights["dm"] = 14; letterPairWeights["dn"] = 16; letterPairWeights["do"] = 41; letterPairWeights["dp"] = 6; letterPairWeights["dq"] = 0; letterPairWeights["dr"] = 14; letterPairWeights["ds"] = 35; letterPairWeights["dt"] = 56; letterPairWeights["du"] = 10; letterPairWeights["dv"] = 2; letterPairWeights["dw"] = 19; letterPairWeights["dx"] = 0; letterPairWeights["dy"] = 10; letterPairWeights["dz"] = 0;
            letterPairWeights["ea"] = 110; letterPairWeights["eb"] = 23; letterPairWeights["ec"] = 45; letterPairWeights["ed"] = 126; letterPairWeights["ee"] = 48; letterPairWeights["ef"] = 30; letterPairWeights["eg"] = 15; letterPairWeights["eh"] = 33; letterPairWeights["ei"] = 41; letterPairWeights["ej"] = 3; letterPairWeights["ek"] = 5; letterPairWeights["el"] = 55; letterPairWeights["em"] = 47; letterPairWeights["en"] = 111; letterPairWeights["eo"] = 33; letterPairWeights["ep"] = 28; letterPairWeights["eq"] = 2; letterPairWeights["er"] = 169; letterPairWeights["es"] = 115; letterPairWeights["et"] = 83; letterPairWeights["eu"] = 6; letterPairWeights["ev"] = 24; letterPairWeights["ew"] = 50; letterPairWeights["ex"] = 9; letterPairWeights["ey"] = 26; letterPairWeights["ez"] = 0;
            letterPairWeights["fa"] = 25; letterPairWeights["fb"] = 2; letterPairWeights["fc"] = 3; letterPairWeights["fd"] = 2; letterPairWeights["fe"] = 20; letterPairWeights["ff"] = 11; letterPairWeights["fg"] = 1; letterPairWeights["fh"] = 8; letterPairWeights["fi"] = 23; letterPairWeights["fj"] = 1; letterPairWeights["fk"] = 0; letterPairWeights["fl"] = 8; letterPairWeights["fm"] = 5; letterPairWeights["fn"] = 1; letterPairWeights["fo"] = 40; letterPairWeights["fp"] = 2; letterPairWeights["fq"] = 0; letterPairWeights["fr"] = 16; letterPairWeights["fs"] = 5; letterPairWeights["ft"] = 37; letterPairWeights["fu"] = 8; letterPairWeights["fv"] = 0; letterPairWeights["fw"] = 3; letterPairWeights["fx"] = 0; letterPairWeights["fy"] = 2; letterPairWeights["fz"] = 0;
            letterPairWeights["ga"] = 24; letterPairWeights["gb"] = 3; letterPairWeights["gc"] = 2; letterPairWeights["gd"] = 2; letterPairWeights["ge"] = 28; letterPairWeights["gf"] = 3; letterPairWeights["gg"] = 4; letterPairWeights["gh"] = 35; letterPairWeights["gi"] = 18; letterPairWeights["gj"] = 1; letterPairWeights["gk"] = 0; letterPairWeights["gl"] = 7; letterPairWeights["gm"] = 3; letterPairWeights["gn"] = 4; letterPairWeights["go"] = 23; letterPairWeights["gp"] = 1; letterPairWeights["gq"] = 0; letterPairWeights["gr"] = 12; letterPairWeights["gs"] = 9; letterPairWeights["gt"] = 16; letterPairWeights["gu"] = 7; letterPairWeights["gv"] = 0; letterPairWeights["gw"] = 5; letterPairWeights["gx"] = 0; letterPairWeights["gy"] = 1; letterPairWeights["gz"] = 0;
            letterPairWeights["ha"] = 114; letterPairWeights["hb"] = 2; letterPairWeights["hc"] = 2; letterPairWeights["hd"] = 1; letterPairWeights["he"] = 302; letterPairWeights["hf"] = 2; letterPairWeights["hg"] = 1; letterPairWeights["hh"] = 3; letterPairWeights["hi"] = 97; letterPairWeights["hj"] = 0; letterPairWeights["hk"] = 0; letterPairWeights["hl"] = 2; letterPairWeights["hm"] = 3; letterPairWeights["hn"] = 1; letterPairWeights["ho"] = 49; letterPairWeights["hp"] = 1; letterPairWeights["hq"] = 0; letterPairWeights["hr"] = 8; letterPairWeights["hs"] = 5; letterPairWeights["ht"] = 32; letterPairWeights["hu"] = 8; letterPairWeights["hv"] = 0; letterPairWeights["hw"] = 4; letterPairWeights["hx"] = 0; letterPairWeights["hy"] = 4; letterPairWeights["hz"] = 0;
            letterPairWeights["ia"] = 10; letterPairWeights["ib"] = 5; letterPairWeights["ic"] = 32; letterPairWeights["id"] = 33; letterPairWeights["ie"] = 23; letterPairWeights["if"] = 17; letterPairWeights["ig"] = 25; letterPairWeights["ih"] = 6; letterPairWeights["ii"] = 1; letterPairWeights["ij"] = 1; letterPairWeights["ik"] = 8; letterPairWeights["il"] = 37; letterPairWeights["im"] = 37; letterPairWeights["in"] = 179; letterPairWeights["io"] = 24; letterPairWeights["ip"] = 6; letterPairWeights["iq"] = 0; letterPairWeights["ir"] = 27;letterPairWeights["is"] = 86; letterPairWeights["it"] = 93; letterPairWeights["iu"] = 1; letterPairWeights["iv"] = 14; letterPairWeights["iw"] = 7; letterPairWeights["ix"] = 2; letterPairWeights["iy"] = 0; letterPairWeights["iz"] = 2;
            letterPairWeights["ja"] = 2; letterPairWeights["jb"] = 0; letterPairWeights["jc"] = 0; letterPairWeights["jd"] = 0; letterPairWeights["je"] = 2; letterPairWeights["jf"] = 0; letterPairWeights["jg"] = 0; letterPairWeights["jh"] = 0; letterPairWeights["ji"] = 3; letterPairWeights["jj"] = 0; letterPairWeights["jk"] = 0; letterPairWeights["jl"] = 0; letterPairWeights["jm"] = 0; letterPairWeights["jn"] = 0; letterPairWeights["jo"] = 3; letterPairWeights["jp"] = 0; letterPairWeights["jq"] = 0; letterPairWeights["jr"] = 0; letterPairWeights["js"] = 0; letterPairWeights["jt"] = 0; letterPairWeights["ju"] = 8; letterPairWeights["jv"] = 0; letterPairWeights["jw"] = 0; letterPairWeights["jx"] = 0; letterPairWeights["jy"] = 0; letterPairWeights["jz"] = 0;
            letterPairWeights["ka"] = 6; letterPairWeights["kb"] = 1; letterPairWeights["kc"] = 1; letterPairWeights["kd"] = 1; letterPairWeights["ke"] = 29; letterPairWeights["kf"] = 1; letterPairWeights["kg"] = 0; letterPairWeights["kh"] = 2; letterPairWeights["ki"] = 14; letterPairWeights["kj"] = 0; letterPairWeights["kk"] = 0; letterPairWeights["kl"] = 2; letterPairWeights["km"] = 1; letterPairWeights["kn"] = 9; letterPairWeights["ko"] = 4; letterPairWeights["kp"] = 0; letterPairWeights["kq"] = 0; letterPairWeights["kr"] = 0; letterPairWeights["ks"] = 5; letterPairWeights["kt"] = 4; letterPairWeights["ku"] = 1; letterPairWeights["kv"] = 0; letterPairWeights["kw"] = 2; letterPairWeights["kx"] = 0; letterPairWeights["ky"] = 2; letterPairWeights["kz"] = 0;
            letterPairWeights["la"] = 40; letterPairWeights["lb"] = 3; letterPairWeights["lc"] = 2; letterPairWeights["ld"] = 36; letterPairWeights["le"] = 64; letterPairWeights["lf"] = 10; letterPairWeights["lg"] = 1; letterPairWeights["lh"] = 4; letterPairWeights["li"] = 47; letterPairWeights["lj"] = 0; letterPairWeights["lk"] = 3; letterPairWeights["ll"] = 56; letterPairWeights["lm"] = 4; letterPairWeights["ln"] = 2; letterPairWeights["lo"] = 41; letterPairWeights["lp"] = 3; letterPairWeights["lq"] = 0; letterPairWeights["lr"] = 2; letterPairWeights["ls"] = 11; letterPairWeights["lt"] = 15; letterPairWeights["lu"] = 8; letterPairWeights["lv"] = 3; letterPairWeights["lw"] = 5; letterPairWeights["lx"] = 0; letterPairWeights["ly"] = 31; letterPairWeights["lz"] = 0;
            letterPairWeights["ma"] = 44; letterPairWeights["mb"] = 7; letterPairWeights["mc"] = 1; letterPairWeights["md"] = 1; letterPairWeights["me"] = 68; letterPairWeights["mf"] = 2; letterPairWeights["mg"] = 1; letterPairWeights["mh"] = 3; letterPairWeights["mi"] = 25; letterPairWeights["mj"] = 0; letterPairWeights["mk"] = 0; letterPairWeights["ml"] = 1; letterPairWeights["mm"] = 5; letterPairWeights["mn"] = 2; letterPairWeights["mo"] = 29; letterPairWeights["mp"] = 11; letterPairWeights["mq"] = 0; letterPairWeights["mr"] = 3; letterPairWeights["ms"] = 10; letterPairWeights["mt"] = 9; letterPairWeights["mu"] = 8; letterPairWeights["mv"] = 0; letterPairWeights["mw"] = 4; letterPairWeights["mx"] = 0; letterPairWeights["my"] = 18; letterPairWeights["mz"] = 0;
            letterPairWeights["na"] = 40; letterPairWeights["nb"] = 7; letterPairWeights["nc"] = 25; letterPairWeights["nd"] = 146; letterPairWeights["ne"] = 66; letterPairWeights["nf"] = 8; letterPairWeights["ng"] = 92; letterPairWeights["nh"] = 16; letterPairWeights["ni"] = 33; letterPairWeights["nj"] = 2; letterPairWeights["nk"] = 8; letterPairWeights["nl"] = 9; letterPairWeights["nm"] = 7; letterPairWeights["nn"] = 8; letterPairWeights["no"] = 60; letterPairWeights["np"] = 4; letterPairWeights["nq"] = 1; letterPairWeights["nr"] = 3; letterPairWeights["ns"] = 33; letterPairWeights["nt"] = 106; letterPairWeights["nu"] = 6; letterPairWeights["nv"] = 2; letterPairWeights["nw"] = 12; letterPairWeights["nx"] = 0; letterPairWeights["ny"] = 11;  letterPairWeights["nz"] = 0;
            letterPairWeights["oa"] = 16; letterPairWeights["ob"] = 12; letterPairWeights["oc"] = 13; letterPairWeights["od"] = 18; letterPairWeights["oe"] = 5; letterPairWeights["of"] = 80; letterPairWeights["og"] = 7; letterPairWeights["oh"] = 11; letterPairWeights["oi"] = 12;  letterPairWeights["oj"] = 1; letterPairWeights["ok"] = 13; letterPairWeights["ol"] = 26;  letterPairWeights["om"] = 48; letterPairWeights["on"] = 106;  letterPairWeights["oo"] = 36;  letterPairWeights["op"] = 15; letterPairWeights["oq"] = 0; letterPairWeights["or"] = 84;  letterPairWeights["os"] = 28; letterPairWeights["ot"] = 57; letterPairWeights["ou"] = 115; letterPairWeights["ov"] = 12; letterPairWeights["ow"] = 46;  letterPairWeights["ox"] = 10; letterPairWeights["oy"] = 5; letterPairWeights["oz"] = 1;
            letterPairWeights["pa"] = 23; letterPairWeights["pb"] = 1; letterPairWeights["pc"] = 0;  letterPairWeights["pd"] = 0; letterPairWeights["pe"] = 30; letterPairWeights["pf"] = 1; letterPairWeights["pg"] = 0; letterPairWeights["ph"] = 3; letterPairWeights["pi"] = 12; letterPairWeights["pj"] = 0; letterPairWeights["pk"] = 0; letterPairWeights["pl"] = 15; letterPairWeights["pm"] = 1; letterPairWeights["pn"] = 0; letterPairWeights["po"] = 21; letterPairWeights["pp"] = 10; letterPairWeights["pq"] = 0; letterPairWeights["pr"] = 18; letterPairWeights["ps"] = 5; letterPairWeights["pt"] = 11; letterPairWeights["pu"] = 6; letterPairWeights["pv"] = 0;  letterPairWeights["pw"] = 1; letterPairWeights["px"] = 0; letterPairWeights["py"] = 1; letterPairWeights["pz"] = 0;
            letterPairWeights["qa"] = 0; letterPairWeights["qb"] = 0; letterPairWeights["qc"] = 0; letterPairWeights["qd"] = 0;  letterPairWeights["qe"] = 0; letterPairWeights["qf"] = 0; letterPairWeights["qg"] = 0; letterPairWeights["qh"] = 0;  letterPairWeights["qi"] = 8; letterPairWeights["qj"] = 0; letterPairWeights["qk"] = 0; letterPairWeights["ql"] = 0; letterPairWeights["qm"] = 0; letterPairWeights["qn"] = 0; letterPairWeights["qo"] = 0; letterPairWeights["qp"] = 0; letterPairWeights["qq"] = 0; letterPairWeights["qr"] = 0; letterPairWeights["qs"] = 0; letterPairWeights["qt"] = 0; letterPairWeights["qu"] = 9; letterPairWeights["qv"] = 0;  letterPairWeights["qw"] = 0; letterPairWeights["qx"] = 0; letterPairWeights["qy"] = 0; letterPairWeights["qz"] = 0;
            letterPairWeights["ra"] = 50;  letterPairWeights["rb"] = 7; letterPairWeights["rc"] = 10; letterPairWeights["rd"] = 20; letterPairWeights["re"] = 133; letterPairWeights["rf"] = 8; letterPairWeights["rg"] = 10; letterPairWeights["rh"] = 12; letterPairWeights["ri"] = 50; letterPairWeights["rj"] = 1; letterPairWeights["rk"] = 8; letterPairWeights["rl"] = 10; letterPairWeights["rm"] = 14; letterPairWeights["rn"] = 16; letterPairWeights["ro"] = 55; letterPairWeights["rp"] = 6; letterPairWeights["rq"] = 0; letterPairWeights["rr"] = 14; letterPairWeights["rs"] = 37; letterPairWeights["rt"] = 42; letterPairWeights["ru"] = 12; letterPairWeights["rv"] = 4; letterPairWeights["rw"] = 11; letterPairWeights["rx"] = 0; letterPairWeights["ry"] = 21; letterPairWeights["rz"] = 0;
            letterPairWeights["sa"] = 67; letterPairWeights["sb"] = 11; letterPairWeights["sc"] = 17; letterPairWeights["sd"] = 7; letterPairWeights["se"] = 74; letterPairWeights["sf"] = 11; letterPairWeights["sg"] = 4; letterPairWeights["sh"] = 50; letterPairWeights["si"] = 49; letterPairWeights["sj"] = 2; letterPairWeights["sk"] = 6; letterPairWeights["sl"] = 13;  letterPairWeights["sm"] = 12; letterPairWeights["sn"] = 10; letterPairWeights["so"] = 57; letterPairWeights["sp"] = 20; letterPairWeights["sq"] = 2; letterPairWeights["sr"] = 4; letterPairWeights["ss"] = 43; letterPairWeights["st"] = 109; letterPairWeights["su"] = 20; letterPairWeights["sv"] = 2; letterPairWeights["sw"] = 24; letterPairWeights["sx"] = 0; letterPairWeights["sy"] = 4; letterPairWeights["sz"] = 0;
            letterPairWeights["ta"] = 59; letterPairWeights["tb"] = 10; letterPairWeights["tc"] = 11; letterPairWeights["td"] = 7; letterPairWeights["te"] = 75; letterPairWeights["tf"] = 9; letterPairWeights["tg"] = 3; letterPairWeights["th"] = 330;  letterPairWeights["ti"] = 76; letterPairWeights["tj"] = 1; letterPairWeights["tk"] = 2; letterPairWeights["tl"] = 17; letterPairWeights["tm"] = 11; letterPairWeights["tn"] = 7; letterPairWeights["to"] = 115; letterPairWeights["tp"] = 4; letterPairWeights["tq"] = 0; letterPairWeights["tr"] = 28; letterPairWeights["ts"] = 34; letterPairWeights["tt"] = 56; letterPairWeights["tu"] = 17; letterPairWeights["tv"] = 1;  letterPairWeights["tw"] = 31; letterPairWeights["tx"] = 0; letterPairWeights["ty"] = 16; letterPairWeights["tz"] = 0;
            letterPairWeights["ua"] = 7;  letterPairWeights["ub"] = 5; letterPairWeights["uc"] = 12; letterPairWeights["ud"] = 7; letterPairWeights["ue"] = 7;  letterPairWeights["uf"] = 2; letterPairWeights["ug"] = 14; letterPairWeights["uh"] = 2; letterPairWeights["ui"] = 8;  letterPairWeights["uj"] = 0; letterPairWeights["uk"] = 1;  letterPairWeights["ul"] = 34; letterPairWeights["um"] = 8;  letterPairWeights["un"] = 36;  letterPairWeights["uo"] = 1;  letterPairWeights["up"] = 16; letterPairWeights["uq"] = 0; letterPairWeights["ur"] = 44;  letterPairWeights["us"] = 35; letterPairWeights["ut"] = 48; letterPairWeights["uu"] = 0; letterPairWeights["uv"] = 0;  letterPairWeights["uw"] = 2; letterPairWeights["ux"] = 0; letterPairWeights["uy"] = 1; letterPairWeights["uz"] = 0;
            letterPairWeights["va"] = 5; letterPairWeights["vb"] = 0; letterPairWeights["vc"] = 0; letterPairWeights["vd"] = 0; letterPairWeights["ve"] = 65; letterPairWeights["vf"] = 0; letterPairWeights["vg"] = 0;letterPairWeights["vh"] = 0; letterPairWeights["vi"] = 11; letterPairWeights["vj"] = 0; letterPairWeights["vk"] = 0; letterPairWeights["vl"] = 0; letterPairWeights["vm"] = 0; letterPairWeights["vn"] = 0; letterPairWeights["vo"] = 4; letterPairWeights["vp"] = 0; letterPairWeights["vq"] = 0;  letterPairWeights["vr"] = 0; letterPairWeights["vs"] = 0; letterPairWeights["vt"] = 0; letterPairWeights["vu"] = 0; letterPairWeights["vv"] = 0; letterPairWeights["vw"] = 0; letterPairWeights["vx"] = 0; letterPairWeights["vy"] = 1; letterPairWeights["vz"] = 0;
            letterPairWeights["wa"] = 66; letterPairWeights["wb"] = 1;  letterPairWeights["wc"] = 1; letterPairWeights["wd"] = 2; letterPairWeights["we"] = 39; letterPairWeights["wf"] = 1;  letterPairWeights["wg"] = 0; letterPairWeights["wh"] = 44; letterPairWeights["wi"] = 39; letterPairWeights["wj"] = 0; letterPairWeights["wk"] = 0; letterPairWeights["wl"] = 2; letterPairWeights["wm"] = 1; letterPairWeights["wn"] = 12; letterPairWeights["wo"] = 29; letterPairWeights["wp"] = 0; letterPairWeights["wq"] = 0; letterPairWeights["wr"] = 3; letterPairWeights["ws"] = 4; letterPairWeights["wt"] = 4; letterPairWeights["wu"] = 1;  letterPairWeights["wv"] = 0; letterPairWeights["ww"] = 2; letterPairWeights["wx"] = 0; letterPairWeights["wy"] = 1; letterPairWeights["wz"] = 0;
            letterPairWeights["xa"] = 1; letterPairWeights["xb"] = 0; letterPairWeights["xc"] = 2; letterPairWeights["xd"] = 0; letterPairWeights["xe"] = 1;  letterPairWeights["xf"] = 0; letterPairWeights["xg"] = 0;  letterPairWeights["xh"] = 0; letterPairWeights["xi"] = 2;letterPairWeights["xj"] = 0; letterPairWeights["xk"] = 0;  letterPairWeights["xl"] = 0; letterPairWeights["xm"] = 0; letterPairWeights["xn"] = 0;  letterPairWeights["xo"] = 0; letterPairWeights["xp"] = 3; letterPairWeights["xq"] = 0; letterPairWeights["xr"] = 0; letterPairWeights["xs"] = 0;  letterPairWeights["xt"] = 3;  letterPairWeights["xu"] = 0; letterPairWeights["xv"] = 0; letterPairWeights["xw"] = 0; letterPairWeights["xx"] = 0; letterPairWeights["xy"] = 0; letterPairWeights["xz"] = 0;
            letterPairWeights["ya"] = 18; letterPairWeights["yb"] = 7; letterPairWeights["yc"] = 6; letterPairWeights["yd"] = 6; letterPairWeights["ye"] = 14; letterPairWeights["yf"] = 7; letterPairWeights["yg"] = 3; letterPairWeights["yh"] = 10; letterPairWeights["yi"] = 11; letterPairWeights["yj"] = 1; letterPairWeights["yk"] = 1; letterPairWeights["yl"] = 4; letterPairWeights["ym"] = 6; letterPairWeights["yn"] = 3; letterPairWeights["yo"] = 36; letterPairWeights["yp"] = 4; letterPairWeights["yq"] = 0; letterPairWeights["yr"] = 3; letterPairWeights["ys"] = 19; letterPairWeights["yt"] = 20;  letterPairWeights["yu"] = 1; letterPairWeights["yv"] = 1; letterPairWeights["yw"] = 12; letterPairWeights["yx"] = 0;  letterPairWeights["yy"] = 2; letterPairWeights["yz"] = 0;
            letterPairWeights["za"] = 1; letterPairWeights["zb"] = 0; letterPairWeights["zc"] = 0; letterPairWeights["zd"] = 0; letterPairWeights["ze"] = 3; letterPairWeights["zf"] = 0; letterPairWeights["zg"] = 0; letterPairWeights["zh"] = 0; letterPairWeights["zi"] = 1; letterPairWeights["zj"] = 0; letterPairWeights["zk"] = 0; letterPairWeights["zl"] = 0; letterPairWeights["zm"] = 0; letterPairWeights["zn"] = 0; letterPairWeights["zo"] = 0; letterPairWeights["zp"] = 0; letterPairWeights["zq"] = 0; letterPairWeights["zr"] = 0; letterPairWeights["zs"] = 0; letterPairWeights["zt"] = 0; letterPairWeights["zu"] = 0; letterPairWeights["zv"] = 0; letterPairWeights["zw"] = 0;letterPairWeights["zx"] = 0; letterPairWeights["zy"] = 0; letterPairWeights["zz"] = 0;

            for(int i = 1; i < 27; i++)
            {
                letterWeights[i] = 0;
            }
            foreach(string key in letterPairWeights.Keys)
            {
                if (key[0].ToString() == "a")
                {
                    letterWeights[1] = letterWeights[1] + letterPairWeights[key];
                }
                else if (key[0].ToString() == "b")
                {
                    letterWeights[2] = letterWeights[2] + letterPairWeights[key];
                }
                else if (key[0].ToString() == "c")
                {
                    letterWeights[3] = letterWeights[3] + letterPairWeights[key];
                }
                else if (key[0].ToString() == "d")
                {
                    letterWeights[4] = letterWeights[4] + letterPairWeights[key];
                }
                else if (key[0].ToString() == "e")
                {
                    letterWeights[5] = letterWeights[5] + letterPairWeights[key];
                }
                else if (key[0].ToString() == "f")
                {
                    letterWeights[6] = letterWeights[6] + letterPairWeights[key];
                }
                else if (key[0].ToString() == "g")
                {
                    letterWeights[7] = letterWeights[7] + letterPairWeights[key];
                }
                else if (key[0].ToString() == "h")
                {
                    letterWeights[8] = letterWeights[8] + letterPairWeights[key];
                }
                else if (key[0].ToString() == "i")
                {
                    letterWeights[9] = letterWeights[9] + letterPairWeights[key];
                }
                else if (key[0].ToString() == "j")
                {
                    letterWeights[10] = letterWeights[10] + letterPairWeights[key];
                }
                else if (key[0].ToString() == "k")
                {
                    letterWeights[11] = letterWeights[11] + letterPairWeights[key];
                }
                else if (key[0].ToString() == "l")
                {
                    letterWeights[12] = letterWeights[12] + letterPairWeights[key];
                }
                else if (key[0].ToString() == "m")
                {
                    letterWeights[13] = letterWeights[13] + letterPairWeights[key];
                }
                else if (key[0].ToString() == "n")
                {
                    letterWeights[14] = letterWeights[14] + letterPairWeights[key];
                }
                else if (key[0].ToString() == "o")
                {
                    letterWeights[15] = letterWeights[15] + letterPairWeights[key];
                }
                else if (key[0].ToString() == "p")
                {
                    letterWeights[16] = letterWeights[16] + letterPairWeights[key];
                }
                else if (key[0].ToString() == "q")
                {
                    letterWeights[17] = letterWeights[17] + letterPairWeights[key];
                }
                else if (key[0].ToString() == "r")
                {
                    letterWeights[18] = letterWeights[18] + letterPairWeights[key];
                }
                else if (key[0].ToString() == "s")
                {
                    letterWeights[19] = letterWeights[19] + letterPairWeights[key];
                }
                else if (key[0].ToString() == "t")
                {
                    letterWeights[20] = letterWeights[20] + letterPairWeights[key];
                }
                else if (key[0].ToString() == "u")
                {
                    letterWeights[21] = letterWeights[21] + letterPairWeights[key];
                }
                else if (key[0].ToString() == "v")
                {
                    letterWeights[22] = letterWeights[22] + letterPairWeights[key];
                }
                else if (key[0].ToString() == "w")
                {
                    letterWeights[23] = letterWeights[23] + letterPairWeights[key];
                }
                else if (key[0].ToString() == "x")
                {
                    letterWeights[24] = letterWeights[24] + letterPairWeights[key];
                }
                else if (key[0].ToString() == "y")
                {
                    letterWeights[25] = letterWeights[25] + letterPairWeights[key];
                }
                else if (key[0].ToString() == "z")
                {
                    letterWeights[26] = letterWeights[26] + letterPairWeights[key];
                }
                totalLetterChance += letterPairWeights[key];
            }
        } 
        private static string GetRandomLetterPair()
        {
            return letterPairList[MathTools.PseudoRandomInt(1, letterPairList.Count)];
        }
        private static string GetNextRandomLetter(string letter)
        {
            List<string> letterList = new List<string>();
            int weight = 0;
            string[] letterPairs = letterPairWeights.Keys.ToArray();
            foreach(string key in letterPairWeights.Keys)
            {
                if (key[0].ToString() == letter)
                {
                    letterList.Add(key);
                    weight += letterPairWeights[key];
                }
            }

            int randLetterInt = MathTools.PseudoRandomInt(1, weight);
            int parse = 0;
            for(int i = 0; i < letterList.Count; i++)
            {
                if (parse >= randLetterInt)
                {
                    
                    return letterList[i][1].ToString();
                }
                else
                {
                    parse += letterPairWeights[letterPairs[i]];
                    randLetterInt -= parse;
                }
            }
            return string.Empty;
        }
        public static string GenerateUniqueNamev2()
        {
            string uniqueName = GetRandomLetterPair();
            int nameLength = MathTools.PseudoRandomInt(4, 10);
            for(int i = 2; i < nameLength; i++)
            {
                if(MathTools.PseudoRandomInt(0,3) == 1)
                {
                    uniqueName += vowels[MathTools.PseudoRandomInt(0, vowels.Count)];
                }
                else
                {
                    uniqueName += GetNextRandomLetter(uniqueName[uniqueName.Length - 1].ToString());
                }
            }

            return PrettifyString(Englishify(Englishify(uniqueName)));
        }
        public static string Englishify(string str)
        {
            str = str.ToLower();
            if (str.Length <= 3) return str;
            int numSinceLastVowel = 0;
            int numVowelsInARow = 0;
            int index = 0;
            foreach(char c in str)
            {
                if (nonVowels.Contains(c.ToString()))
                {
                    numSinceLastVowel++;
                }
                else
                {
                    numVowelsInARow++;
                }
                if(numSinceLastVowel > 1 && index < str.Length - 1)
                {
                    str = str.Substring(0, index) + vowels[MathTools.PseudoRandomInt(0, vowels.Count)] + str.Substring(index);
                    numVowelsInARow++;
                    numSinceLastVowel = 0;
                }
                if(numVowelsInARow > 2 && index < str.Length - 1)
                {
                    str = str.Remove(index-1, 1);
                    numVowelsInARow = 0;
                }
                if (index > 1 && index < str.Length)
                {
                    string checkPair = str[index - 1].ToString() + str[index].ToString();
                    if (letterPairWeights[checkPair] <= 7)
                    {
                        str = str.Remove(index-1, 1);
                    }
                }
                else if ( index == 0 && index < str.Length)
                {
                    string checkPair = str[0].ToString() + str[1].ToString();
                    if (letterPairWeights[checkPair] <= 7)
                    {
                        str = str.Substring(1);
                    }
                }
                index++;
            }
            int numVowels = 0;
            foreach(char c in str)
            {
                if (vowels.Contains(c.ToString()))
                {
                    numVowels++;
                }
            }
            if(numVowels == 0 && str.Length >= 2)
            {
                str = str.Substring(0, 1) + vowels[MathTools.PseudoRandomInt(0, vowels.Count)] + str.Substring(2);
            }
            if(str.Length <= 2)
            {
                str += vowels[MathTools.PseudoRandomInt(0, vowels.Count)];
            }
            return str;
        }

        public static string PrettifyString(string str)
        {

            return char.ToUpper(str[0]) + str.Substring(1);
        }
    }
    public enum NameType
    {
        Faction, TradeStation
    }

}
