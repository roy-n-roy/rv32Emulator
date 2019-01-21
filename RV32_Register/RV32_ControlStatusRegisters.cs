using RV32_Register.Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RV32_Register {
    [Serializable]
    public class RV32_ControlStatusRegisters : IDictionary<CSR, UInt32> {

        private Dictionary<CSR, UInt32> csr;

        public UInt32 Misa { get => csr[CSR.misa]; set => csr[CSR.misa] = value; }

        public RV32_ControlStatusRegisters(IDictionary<CSR, UInt32> dictionary) {
            csr = new Dictionary<CSR, uint>(dictionary);
        }

        public Dictionary<CSR, UInt32> GetDictionary() => csr;

        public UInt32 this[CSR name] {
            get {
                switch (name) {
                    // sstatus,ustatus
                    // 一部の下位レベルCSRへのアクセスは、制限された上位レベルCSRへのアクセスとして読み替える
                    case CSR.sstatus:
                        return csr[CSR.mstatus] & StatusCSR.SModeMask;

                    case CSR.ustatus:
                        return csr[CSR.mstatus] & StatusCSR.UModeMask;

                    // sip,uip
                    case CSR.sip:
                        return csr[CSR.mip] & InterruptPendingCSR.SModeReadMask;

                    case CSR.uip:
                        return csr[CSR.mip] & InterruptPendingCSR.UModeReadMask;

                    // sie,sideleg,uie,uideleg
                    case CSR.sie:
                        return csr[CSR.mie] & InterruptEnableCSR.SModeMask;

                    case CSR.uie:
                        return csr[CSR.mie] & InterruptEnableCSR.UModeMask;

                    // fflags,frm
                    // fcsrへのアクセスとして読み替える
                    case CSR.fflags:
                        return csr[CSR.fcsr] & FloatCSR.FflagsMask;

                    case CSR.frm:
                        return (csr[CSR.fcsr] & FloatCSR.FrmMask) >> 5;

                    // mtvec,stvec,utvec
                    // ベクタードモードかつ割り込みの場合は、ベースアドレス+(cause * 4)として読み替える
                    case CSR.mtvec:
                        TvecCSR tvec;
                        tvec = csr[CSR.mtvec];
                        return (tvec.MODE == 1 && (csr[CSR.mcause] & 0x8000_0000U) == 0x8000_0000U) ? tvec.BASE + (csr[CSR.mcause] * 4) : tvec.BASE;

                    case CSR.stvec:
                        tvec = csr[CSR.stvec];
                        return (tvec.MODE == 1 && (csr[CSR.scause] & 0x8000_0000U) == 0x8000_0000U) ? tvec.BASE + (csr[CSR.scause] * 4) : tvec.BASE;

                    case CSR.utvec:
                        tvec = csr[CSR.utvec];
                        return (tvec.MODE == 1 && (csr[CSR.ucause] & 0x8000_0000U) == 0x8000_0000U) ? tvec.BASE + (csr[CSR.ucause] * 4) : tvec.BASE;

                    default:
                        // cycle,time,insret,hpmcounter3～31
                        // mcycle,mtime,minsret,mhpcounterの読み込み専用シャドウとしてアクセスする
                        if ((CSR.cycle <= name && name <= CSR.hpmcounter31) || (CSR.cycleh <= name && name <= CSR.hpmcounter31h)) {
                            return csr[CSR.mcycle | (name & (CSR)0xffu)];

                        } else {
                            return csr[name];
                        }
                }

            }
            set {
                switch (name) {
                    // mstatus,sstatus,ustatus
                    case CSR.mstatus:
                        // 一部の下位レベルCSRへのアクセスは、制限された上位レベルCSRへのアクセスとして読み替える
                        // またstatusのMPP, SPPはサポートしている特権モードだけを格納する
                        StatusCSR status;
                        status = value;
                        if ((csr[CSR.misa] & (1U << ('S' - 'A'))) == 0) {
                            // スーパーバイザモードをサポートしていない場合
                            status.MPP = 0b11;
                        } else if ((csr[CSR.misa] & (1U << ('U' - 'A'))) == 0) {
                            // ユーザモードをサポートしていない場合
                            status.MPP = 0b01;
                            status.SPP = true;
                        }
                        csr[CSR.mstatus] = status;
                        break;

                    case CSR.sstatus:
                        status = value;
                        if ((csr[CSR.misa] & (1U << ('U' - 'A'))) == 0) {
                            // ユーザモードをサポートしていない場合
                            status.SPP = true;
                        }
                        csr[CSR.mstatus] = csr[CSR.mstatus] & ~StatusCSR.SModeMask | status & StatusCSR.SModeMask;
                        break;

                    case CSR.ustatus:
                        csr[CSR.mstatus] = csr[CSR.mstatus] & ~StatusCSR.UModeMask | value & StatusCSR.UModeMask;
                        break;

                    // mip,sip,uip
                    case CSR.mip:
                        csr[CSR.mip] = csr[CSR.mip] & ~InterruptPendingCSR.MModeWriteMask | value & InterruptPendingCSR.MModeWriteMask;
                        break;

                    case CSR.sip:
                        csr[CSR.mip] = csr[CSR.mip] & ~InterruptPendingCSR.SModeWriteMask | value & InterruptPendingCSR.SModeWriteMask;
                        break;

                    case CSR.uip:
                        csr[CSR.mip] = csr[CSR.mip] & ~InterruptPendingCSR.UModeWriteMask | value & InterruptPendingCSR.UModeWriteMask;
                        break;

                    // sie,uie
                    case CSR.sie:
                        csr[CSR.mie] = csr[CSR.mie] & ~InterruptEnableCSR.SModeMask | value & InterruptEnableCSR.SModeMask;
                        break;

                    case CSR.uie:
                        csr[CSR.mie] = csr[CSR.mie] & ~InterruptEnableCSR.UModeMask | value & InterruptEnableCSR.UModeMask;
                        break;

                    // mepc,sepc,uepc
                    // epcの下位2bitは常にゼロとなる
                    case CSR.mepc:
                    case CSR.sepc:
                    case CSR.uepc:
                        csr[name] = value & 0xffff_fffc;
                        break;

                    // fcsr,fflags,frm
                    // fcsrへのアクセスとして読み替える
                    case CSR.fcsr:
                        csr[CSR.fcsr] = value & (FloatCSR.FflagsMask | FloatCSR.FrmMask);
                        status = new StatusCSR { FS = 0x3 };
                        csr[CSR.mstatus] |= status;
                        break;

                    case CSR.fflags:
                        csr[CSR.fcsr] = csr[CSR.fcsr] & ~FloatCSR.FflagsMask | value & FloatCSR.FflagsMask;
                        status = new StatusCSR { FS = 0x3 };
                        csr[CSR.mstatus] |= status;
                        break;

                    case CSR.frm:
                        csr[CSR.fcsr] = csr[CSR.fcsr] & ~FloatCSR.FrmMask | (value << 5) & FloatCSR.FrmMask;
                        status = new StatusCSR { FS = 0x3 };
                        csr[CSR.mstatus] |= status;
                        break;

                    // misa
                    // misaへの書き込みは無視する
                    case CSR.misa:
                        break;

                    default:
                        csr[name] = value;
                        break;
                }
            }
        }

        ICollection<CSR> IDictionary<CSR, uint>.Keys => this.Keys;
        ICollection<uint> IDictionary<CSR, uint>.Values => this.Values;
        protected ICollection<CSR> Keys { get => csr.Keys; }
        protected ICollection<uint> Values { get => csr.Values; }
        public bool IsReadOnly => false;
        public bool IsFixedSize => true;
        public int Count => csr.Count();
        public object SyncRoot => this;
        public bool IsSynchronized => false;
        public bool Contains(KeyValuePair<CSR, uint> item) => csr.Contains(item);
        public void Add(CSR key, UInt32 value) { }
        public void Add(KeyValuePair<CSR, uint> item) { }
        public void Clear() { }
        public IDictionaryEnumerator GetEnumerator() => csr.GetEnumerator();
        public void Remove(CSR key) { }
        public void CopyTo(Array array, int index) => csr.ToArray().CopyTo(array, index);
        IEnumerator IEnumerable.GetEnumerator() => csr.GetEnumerator();
        public bool ContainsKey(CSR key) => csr.ContainsKey(key);
        bool IDictionary<CSR, uint>.Remove(CSR key) => csr.Remove(key);
        public bool TryGetValue(CSR key, out uint value) => csr.TryGetValue(key, out value);
        public void CopyTo(KeyValuePair<CSR, uint>[] array, int arrayIndex) => csr.ToArray().CopyTo(array, arrayIndex);
        public bool Remove(KeyValuePair<CSR, uint> item) => csr.Remove(item.Key);
        IEnumerator<KeyValuePair<CSR, uint>> IEnumerable<KeyValuePair<CSR, uint>>.GetEnumerator() => csr.GetEnumerator();
    }
}
