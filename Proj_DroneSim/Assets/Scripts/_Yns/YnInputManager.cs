using UnityEngine;

namespace Yns
{
    public class YnInputManager : MonoBehaviour
    {
        static float m_LPadH = 0.0f;
        static float m_LPadV = 0.0f;
        static float m_LPadHo = 0.0f;
        static float m_LPadVo = 0.0f;
        static float m_RPadH = 0.0f;
        static float m_RPadV = 0.0f;
        static float m_RPadHo = 0.0f;
        static float m_RPadVo = 0.0f;

        //
        private void Awake()
        {
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            DPadCtrl();
        }

        private void DPadCtrl()
        {
            m_LPadH = Input.GetAxis("L_Pad_H");
            m_LPadV = Input.GetAxis("L_Pad_V");
            m_RPadH = Input.GetAxis("R_Pad_H");
            m_RPadV = Input.GetAxis("R_Pad_V");

            bool isClearLph = false;
            bool isClearLpv = false;
            bool isClearDph = false;
            bool isClearDpv = false;

            if (m_LPadH != 0.0f && m_LPadHo != 0.0f)
            {
                if (m_LPadH > 0.0f && m_LPadHo > 0.0f)
                {
                    isClearLph = true;
                }
                else
                if (m_LPadH < 0.0f && m_LPadHo < 0.0f)
                {
                    isClearLph = true;
                }
            }
            if (m_LPadV != 0.0f && m_LPadVo != 0.0f)
            {
                if (m_LPadV > 0.0f && m_LPadVo > 0.0f)
                {
                    isClearLpv = true;
                }
                else
                if (m_LPadV < 0.0f && m_LPadVo < 0.0f)
                {
                    isClearLpv = true;
                }
            }
            m_LPadHo = m_LPadH;
            m_LPadVo = m_LPadV;
            if (isClearLph)
            {
                m_LPadH = 0.0f;
            }
            if (isClearLpv)
            {
                m_LPadV = 0.0f;
            }

            if (m_RPadH != 0.0f && m_RPadHo != 0.0f)
            {
                if (m_RPadH > 0.0f && m_RPadHo > 0.0f)
                {
                    isClearDph = true;
                }
                else
                if (m_RPadH < 0.0f && m_RPadHo < 0.0f)
                {
                    isClearDph = true;
                }
            }
            if (m_RPadV != 0.0f && m_RPadVo != 0.0f)
            {
                if (m_RPadV > 0.0f && m_RPadVo > 0.0f)
                {
                    isClearDpv = true;
                }
                else
                if (m_RPadV < 0.0f && m_RPadVo < 0.0f)
                {
                    isClearDpv = true;
                }
            }
            m_RPadHo = m_RPadH;
            m_RPadVo = m_RPadV;
            if (isClearDph)
            {
                m_RPadH = 0.0f;
            }
            if (isClearDpv)
            {
                m_RPadV = 0.0f;
            }

        }

        public static bool IsTrgLPadLeft()
        {
            return m_LPadH < 0.0f;
        }

        public static bool IsTrgLPadRight()
        {
            return m_LPadH > 0.0f;
        }

        public static bool IsTrgLPadUp()
        {
            return m_LPadV > 0.0f;
        }

        public static bool IsTrgLPadDown()
        {
            return m_LPadV < 0.0f;
        }

        public static bool IsTrgRPadLeft()
        {
            return m_RPadH < 0.0f;
        }

        public static bool IsTrgRPadRight()
        {
            return m_RPadH > 0.0f;
        }

        public static bool IsTrgRPadUp()
        {
            return m_RPadV > 0.0f;
        }

        public static bool IsTrgRPadDown()
        {
            return m_RPadV < 0.0f;
        }
    }
}
