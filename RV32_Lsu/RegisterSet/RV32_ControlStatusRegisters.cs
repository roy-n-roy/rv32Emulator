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
                        base[CSR.mstatus] = value & StatusCSR.SModeMask;
                        break;

                    case CSR.ustatus:
                        base[CSR.mstatus] = value & StatusCSR.UModeMask;
                        break;

                    // sip,uip
                    case CSR.sip:
                        base[CSR.mip] = value & InterruptPendingCSR.SModeMask;
                        break;

                    case CSR.uip:
                        base[CSR.mip] = value & InterruptPendingCSR.UModeMask;
                        break;

                    // sie,uie
                    case CSR.sie:
                        base[CSR.mie] = value & InterruptEnableCSR.SModeMask;
                        break;

                    case CSR.uie:
                        base[CSR.mie] = value & InterruptEnableCSR.UModeMask;
                        break;

                    default:
                        base[name] = value;
                        break;
                }
        }
    }
    }
}
