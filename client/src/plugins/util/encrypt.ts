// 利用 Crypto.js 计算哈希值

import CryptoJS from 'crypto-js'
import sha256 from 'crypto-js/sha256'

export function calculate_sha256(input: string): string {
  return sha256(input).toString(CryptoJS.enc.Hex)
}