using System;
using LeagueSharp;
using SharpDX;
using SharpDX.Direct3D9;

namespace TAC_Jax
{
    /**
     * @author Detuks
     * */
    class HpBarIndicator
    {

        public static SharpDX.Direct3D9.Device DxDevice = Drawing.Direct3DDevice;
        public static SharpDX.Direct3D9.Line DxLine;

        public Obj_AI_Hero Unit { get; set; }

        public float Width = 104;

        public float Hight = 9;


        public HpBarIndicator()
        {
            DxLine = new Line(DxDevice) { Width = 9 };

            Drawing.OnPreReset += DrawingOnOnPreReset;
            Drawing.OnPostReset += DrawingOnOnPostReset;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomainOnDomainUnload;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnDomainUnload;

        }


        private static void CurrentDomainOnDomainUnload(object sender, EventArgs eventArgs)
        {
            DxLine.Dispose();
        }

        private static void DrawingOnOnPostReset(EventArgs args)
        {
            DxLine.OnResetDevice();
        }

        private static void DrawingOnOnPreReset(EventArgs args)
        {
            DxLine.OnLostDevice();
        }

        private Vector2 Offset
        {
            get
            {
                if (Unit != null)
                {
                    return Unit.IsAlly ? new Vector2(34, 9) : new Vector2(10, 20);
                }

                return new Vector2();
            }
        }

        public Vector2 StartPosition
        {

            get { return new Vector2(Unit.HPBarPosition.X + Offset.X, Unit.HPBarPosition.Y + Offset.Y); }
        }


        private float GetHpProc(float dmg = 0)
        {
            float health = ((Unit.Health - dmg) > 0) ? (Unit.Health - dmg) : 0;
            return (health / Unit.MaxHealth);
        }

        private Vector2 GetHpPosAfterDmg(float dmg)
        {
            float w = GetHpProc(dmg) * Width;
            return new Vector2(StartPosition.X + w, StartPosition.Y);
        }

        public void DrawDmg(float dmg, System.Drawing.Color color)
        {
            var hpPosNow = GetHpPosAfterDmg(0);
            var hpPosAfter = GetHpPosAfterDmg(dmg);

            fillHPBar(hpPosNow, hpPosAfter, color);
            //fillHPBar((int)(hpPosNow.X - startPosition.X), (int)(hpPosAfter.X- startPosition.X), color);
        }

        private void fillHPBar(int to, int from, System.Drawing.Color color)
        {
            Vector2 sPos = StartPosition;

            for (int i = from; i < to; i++)
            {
                Drawing.DrawLine(sPos.X + i, sPos.Y, sPos.X + i, sPos.Y + 9, 1, color);
            }
        }

        private void fillHPBar(Vector2 from, Vector2 to, System.Drawing.Color color)
        {
            DxLine.Begin();

            DxLine.Draw(new[]
                                    {
                                        new Vector2((int)from.X, (int)from.Y + 4f),
                                        new Vector2( (int)to.X, (int)to.Y + 4f)
                                    }, new ColorBGRA(255, 255, 00, 90));
            // Vector2 sPos = startPosition;
            //Drawing.DrawLine((int)from.X, (int)from.Y + 9f, (int)to.X, (int)to.Y + 9f, 9f, color);

            DxLine.End();
        }

    }
}