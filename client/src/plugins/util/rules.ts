export const notNullRule = (value: string) => {
  if (value !== '') {
    return true;
  } else {
    return '请填入信息';
  }
}

export const isPositiveIntegerRule = (value: string) => {
  if (/^(0|[1-9]\d*)$/.test(value)) {
    return true;
  } else {
    return '请输入正整数！';
  }
}

export const isNonNegativeIntegerRule = (value: string) => {
  if (/^(0|[1-9]\d*)$/.test(value)) {
    return true;
  } else {
    return '请输入正整数！';
  }
}
