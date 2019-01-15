using RV32_Register.Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RV32_Register {
    public class RV32_ControlStatusRegisters : Dictionary<CSR, UInt32> {

        public UInt32 Misa { get => base[CSR.misa]; set => base[CSR.misa] = value; }

        public RV32_ControlStatusRegisters(IDictionary<CSR, uint> dictionary) : base(dictionary) {
        }

        public new UInt32 this[CSR name] {
            get {
                switch (name) {
                    // sstatus,ustatus
                    // 一部の下位レベルCSRへのアクセスは、制限された上位レベルCSRへのアクセスとして読み替える
                    case CSR.sstatus:
                        return base[CSR.mstatus] & StatusCSR.SModeMask;

                    case CSR.ustatus:
                        return base[CSR.mstatus] & StatusCSR.UModeMask;

                    // sip,uip
                    case CSR.sip:
                        return base[CSR.mip] & InterruptPendingCSR.SModeReadMask;

                    case CSR.uip:
                        return base[CSR.mip] & InterruptPendingCSR.UModeReadMask;

                    // sie,sideleg,uie,uideleg
                    case CSR.sie:
                        return base[CSR.mie] & InterruptEnableCSR.SModeMask;

                    case CSR.uie:
                        return base[CSR.mie] & InterruptEnableCSR.UModeMask;

                    // fflags,frm
                    // fcsrへのアクセスとして読み替える
                    case CSR.fflags:
                        return base[CSR.fcsr] & FloatCSR.FflagsMask;

                    case CSR.frm:
                        return (base[CSR.fcsr] & FloatCSR.FrmMask) >> 5;

                    // mtvec,stvec,utvec
                    // ベクタードモードかつ割り込みの場合は、ベースアドレス+(cause * 4)として読み替える
                    case CSR.mtvec:
                        TvecCSR tvec;
                        tvec = base[CSR.mtvec];
                        return (tvec.MODE == 1 && (base[CSR.mcause] & 0x8000_0000u) == 0x8000_0000u) ? tvec.BASE + (base[CSR.mcause] * 4) : tvec.BASE;

                    case CSR.stvec:
                        tvec = base[CSR.stvec];
                        return (tvec.MODE == 1 && (base[CSR.scause] & 0x8000_0000u) == 0x8000_0000u) ? tvec.BASE + (base[CSR.scause] * 4) : tvec.BASE;

                    case CSR.utvec:
                        tvec = base[CSR.utvec];
                        return (tvec.MODE == 1 && (base[CSR.ucause] & 0x8000_0000u) == 0x8000_0000u) ? tvec.BASE + (base[CSR.ucause] * 4) : tvec.BASE;

                    default:
                        // cycle,time,insret,hpmcounter3～31
                        // mcycle,mtime,minsret,mhpcounterの読み込み専用シャドウとしてアクセスする
                        if ((CSR.cycle <= name && name <= CSR.hpmcounter31) || (CSR.cycleh <= name && name <= CSR.hpmcounter31h)) {
                            return base[CSR.mcycle | (name & (CSR)0xffu)];

                        } else {
                            return base[name];
                        }
                }

            }
            set {
                switch (name) {
                    // sstatus,ustatus
                    // 一部の下位レベルCSRへのアクセスは、制限された上位レベルCSRへのアクセスとして読み替える
                    case CSR.sstatus:
                        base[CSR.mstatus] = base[CSR.mstatus] & ~StatusCSR.SModeMask | value & StatusCSR.SModeMask;
                        break;

                    case CSR.ustatus:
                        base[CSR.mstatus] = base[CSR.mstatus] & ~StatusCSR.UModeMask | value & StatusCSR.UModeMask;
                        break;

                    // mip,sip,uip
                    case CSR.mip:
                        base[CSR.mip] = base[CSR.mip] & ~InterruptPendingCSR.MModeWriteMask | value & InterruptPendingCSR.MModeWriteMask;
                        break;

                    case CSR.sip:
                        base[CSR.mip] = base[CSR.mip] & ~InterruptPendingCSR.SModeWriteMask | value & InterruptPendingCSR.SModeWriteMask;
                        break;

                    case CSR.uip:
                        base[CSR.mip] = base[CSR.mip] & ~InterruptPendingCSR.UModeWriteMask | value & InterruptPendingCSR.UModeWriteMask;
                        break;

                    // sie,uie
                    case CSR.sie:
                        base[CSR.mie] = base[CSR.mie] & ~InterruptEnableCSR.SModeMask | value & InterruptEnableCSR.SModeMask;
                        break;

                    case CSR.uie:
                        base[CSR.mie] = base[CSR.mie] & ~InterruptEnableCSR.UModeMask | value & InterruptEnableCSR.UModeMask;
                        break;

                    // mepc,sepc,uepc
                    // epcの下位2bitは常にゼロとなる
                    case CSR.mepc:
                    case CSR.sepc:
                    case CSR.uepc:
                        base[name] = value & 0xffff_fffc;
                        break;

                    // fcsr,fflags,frm
                    // fcsrへのアクセスとして読み替える
                    case CSR.fcsr:
                        base[CSR.fcsr] =  value & (FloatCSR.FflagsMask | FloatCSR.FrmMask);
                        StatusCSR status;
                        status = new StatusCSR { FS = 0x3 };
                        base[CSR.mstatus] |= status;
                        break;

                    case CSR.fflags:
                        base[CSR.fcsr] = base[CSR.fcsr] & ~FloatCSR.FflagsMask | value & FloatCSR.FflagsMask;
                        status = new StatusCSR { FS = 0x3 };
                        base[CSR.mstatus] |= status;
                        break;

                    case CSR.frm:
                        base[CSR.fcsr] = base[CSR.fcsr] & ~FloatCSR.FrmMask | (value << 5) & FloatCSR.FrmMask;
                        status = new StatusCSR { FS = 0x3 };
                        base[CSR.mstatus] |= status;
                        break;

                    // misa
                    // misaへの書き込みは無視する
                    case CSR.misa:
                        break;

                    default:
                        base[name] = value;
                        break;
                }
        }
    }
    }
}
