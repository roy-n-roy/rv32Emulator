using RiscVCpu.LoadStoreUnit.Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiscVCpu.RegisterSet {
    public class RV32_ControlStatusRegisters : Dictionary<CSR, UInt32> {
        public RV32_ControlStatusRegisters(IDictionary<CSR, uint> dictionary) : base(dictionary) {
        }

        public new UInt32 this[CSR name] {
            get {
                switch (name) {
                    // 一部の下位レベルCSRへのアクセスは、制限された上位レベルCSRへのアクセスとして読み替える
                    // sstatus,ustatus
                    case CSR.sstatus:
                        return base[CSR.mstatus] & StatusCSR.SModeMask;

                    case CSR.ustatus:
                        return base[CSR.mstatus] & StatusCSR.UModeMask;

                    // sip,uip
                    case CSR.sip:
                        return base[CSR.mip] & InterruptPendingCSR.SModeMask;

                    case CSR.uip:
                        return base[CSR.mip] & InterruptPendingCSR.UModeMask;

                    // sie,uie
                    case CSR.sie:
                        return base[CSR.mie] & InterruptEnableCSR.SModeMask;

                    case CSR.uie:
                        return base[CSR.mie] & InterruptEnableCSR.UModeMask;

                    // fflags,frm
                    case CSR.fflags:
                        return base[CSR.fcsr] & FloatCSR.FflagsMask;

                    case CSR.frm:
                        return (base[CSR.fcsr] & FloatCSR.FrmMask) >> 5;

                    default:
                        // cycle,time,insret,hpmcounter3～31
                        if ((CSR.cycle <= name && name <= CSR.hpmcounter31) || (CSR.cycleh <= name && name <= CSR.hpmcounter31h)) {
                            return base[(name & (CSR)0xffu) | CSR.mcycle];

                        } else {
                            return base[name];
                        }
                }

            }
            set {
                    // 一部の下位レベルCSRへのアクセスは、制限された上位レベルCSRへのアクセスとして読み替える
                switch (name) {
                    case CSR.sstatus:
                        base[CSR.mstatus] = base[CSR.mstatus] & ~StatusCSR.SModeMask | value & StatusCSR.SModeMask;
                        break;

                    case CSR.ustatus:
                        base[CSR.mstatus] = base[CSR.mstatus] & ~StatusCSR.UModeMask | value & StatusCSR.UModeMask;
                        break;

                    // sip,uip
                    case CSR.sip:
                        base[CSR.mip] = base[CSR.mip] & ~InterruptPendingCSR.SModeMask | value & InterruptPendingCSR.SModeMask;
                        break;

                    case CSR.uip:
                        base[CSR.mip] = base[CSR.mip] & ~InterruptPendingCSR.UModeMask | value & InterruptPendingCSR.UModeMask;
                        break;

                    // sie,uie
                    case CSR.sie:
                        base[CSR.mie] = base[CSR.mie] & ~InterruptEnableCSR.SModeMask | value & InterruptEnableCSR.SModeMask;
                        break;

                    case CSR.uie:
                        base[CSR.mie] = base[CSR.mie] & ~InterruptEnableCSR.UModeMask | value & InterruptEnableCSR.UModeMask;
                        break;

                    // fcsr,fflags,frm
                    case CSR.fcsr:
                        base[CSR.fcsr] =  value & (FloatCSR.FflagsMask | FloatCSR.FrmMask);
                        break;

                    case CSR.fflags:
                        base[CSR.fcsr] = base[CSR.fcsr] & ~FloatCSR.FflagsMask | value & FloatCSR.FflagsMask;
                        break;

                    case CSR.frm:
                        base[CSR.fcsr] = base[CSR.fcsr] & ~FloatCSR.FrmMask | (value << 5) & FloatCSR.FrmMask;
                        break;

                    default:
                        base[name] = value;
                        break;
                }
        }
    }
    }
}
