// 获取 Vanta.js clouds 效果的颜色的模块

// 插值代码来自：https://www.zhihu.com/question/38869928/answer/78527903

// HTML颜色转RGB颜色
function parseColor(hexString: string) {
  return [
    hexString.substring(1, 3),
    hexString.substring(3, 5),
    hexString.substring(5, 7),
  ].map((s) => { return parseInt(s, 16); })
};

// 将一位补位到两位
function pad(s: string) {
  return (s.length === 1) ? '0' + s : s;
};

// 插值获得颜色
function gradientColors(startHTML: string, endHTML: string, steps: number, gamma: number) {
  let i, j, ms, me, output = [], so = [];
  gamma = gamma || 1;
  const normalize = (channel: number) => {
    return Math.pow(channel / 255, gamma);
  };
  const startRGB = parseColor(startHTML).map(normalize);
  const endRGB = parseColor(endHTML).map(normalize);
  for (i = 0; i < steps; i++) {
    ms = i / (steps - 1);
    me = 1 - ms;
    for (j = 0; j < 3; j++) {
      so[j] = pad(Math.round(Math.pow(startRGB[j] * me + endRGB[j] * ms, 1 / gamma) * 255).toString(16));
    }
    output.push('#' + so.join(''));
  }
  return output;
};

// 获取系统时间，从0点开始计数，以分钟为单位
function getCurrMinute(): number {
  const now = new Date()
  return now.getHours() * 60 + now.getMinutes()
}

export function getSkyColor(): string {
  const black = '#121212'
  const purple = '#140248'
  const orange = '#fea443'
  const dusk = '#c88a46'
  const blue = '#58acfa'

  const currMinute = getCurrMinute()
  let currColor = black

  if (currMinute >= 0 && currMinute < 4 * 60) {
    currColor = black
  } else if (currMinute >= 4 * 60 && currMinute < 5 * 60) {
    currColor = gradientColors(black, purple, 60, 1)[currMinute - 4 * 60]
  } else if (currMinute >= 5 * 60 && currMinute < 6 * 60) {
    currColor = gradientColors(purple, orange, 60, 1)[currMinute - 5 * 60]
  } else if (currMinute >= 6 * 60 && currMinute < 9 * 60) {
    currColor = gradientColors(orange, blue, 3 * 60, 1)[currMinute - 6 * 60]
  } else if (currMinute >= 9 * 60 && currMinute < 16 * 60) {
    currColor = blue
  } else if (currMinute >= 16 * 60 && currMinute < 18 * 60) {
    currColor = gradientColors(blue, dusk, 2 * 60, 1)[currMinute - 16 * 60]
  } else if (currMinute >= 18 * 60 && currMinute < 19 * 60) {
    currColor = gradientColors(dusk, purple, 60, 1)[currMinute - 18 * 60]
  } else if (currMinute >= 19 * 60 && currMinute < 20 * 60) {
    currColor = gradientColors(purple, black, 60, 1)[currMinute - 19 * 60]
  } else if (currMinute >= 20 * 60 && currMinute < 24 * 60) {
    currColor = black
  }
  return currColor
}

export function getCloudColor(): string {
  const grey = '#525252'
  const whiteblue= '#adc1de'

  const currMinute = getCurrMinute()
  let currColor = whiteblue

  if (currMinute >= 0 && currMinute < 4 * 60) {
    currColor = grey
  } else if (currMinute >= 4 * 60 && currMinute < 6 * 60) {
    currColor = gradientColors(grey, whiteblue, 2 * 60, 1)[currMinute - 4 * 60]
  } else if (currMinute >= 6 * 60 && currMinute < 17 * 60) {
    currColor = whiteblue
  } else if (currMinute >= 17 * 60 && currMinute < 19 * 60) {
    currColor = gradientColors(whiteblue, grey, 2 * 60, 1)[currMinute - 17 * 60]
  } else if (currMinute >= 19 * 60 && currMinute < 24 * 60) {
    currColor = grey
  }

  return currColor
}

export function getSunColor(): string {
  const white = '#fef0c0'
  const lightorange = '#f5d0a9'
  const black = '#000000'

  const currMinute = getCurrMinute()
  let currColor = white

  if (currMinute >= 0 && currMinute < 4 * 60) {
    currColor = black
  } else if (currMinute >= 4 && currMinute < 6 * 60) {
    currColor = gradientColors(black, lightorange, 2 * 60, 1)[currMinute - 4 * 60]
  } else if (currMinute >= 6 * 60 && currMinute < 8 * 60) {
    currColor = gradientColors(lightorange, white, 2 * 60, 1)[currMinute - 6 * 60]
  } else if (currMinute >= 8 * 60 && currMinute < 16 * 60) {
    currColor = white
  } else if (currMinute >= 16 * 60 && currMinute < 18 * 60) {
    currColor = gradientColors(white, lightorange, 2 * 60, 1)[currMinute - 16 * 60]
  } else if (currMinute >= 18 * 60 && currMinute < 19 * 60) {
    currColor = gradientColors(lightorange, black, 60, 1)[currMinute - 18 * 60]
  } else if (currMinute >= 19 * 60 && currMinute < 24 * 60) {
    currColor = black
  }

  return currColor
}

export function getSunGlareColor(): string {
  const white = '#fef0c0'
  const orange = '#ffa64d'
  const black = '#000000'

  const currMinute = getCurrMinute()
  let currColor = white

  if (currMinute >= 0 && currMinute < 4 * 60) {
    currColor = black
  } else if (currMinute >= 4 && currMinute < 6 * 60) {
    currColor = gradientColors(black, orange, 2 * 60, 1)[currMinute - 4 * 60]
  } else if (currMinute >= 6 * 60 && currMinute < 7 * 60) {
    currColor = gradientColors(orange, white, 60, 1)[currMinute - 6 * 60]
  } else if (currMinute >= 7 * 60 && currMinute < 17 * 60) {
    currColor = white
  } else if (currMinute >= 17 * 60 && currMinute < 18 * 60) {
    currColor = gradientColors(white, orange, 60, 1)[currMinute - 17 * 60]
  } else if (currMinute >= 18 * 60 && currMinute < 19 * 60) {
    currColor = gradientColors(orange, black, 60, 1)[currMinute - 18 * 60]
  } else if (currMinute >= 19 * 60 && currMinute < 24 * 60) {
    currColor = black
  }

  return currColor
}

export function getRandomColorList(count: number): string[] {
  const list = [
    "#FFCDD2", "#F8BBD0", "#E1BEE7", "#D1C4E9", "#C5CAE9", "#BBDEFB",
    "#B3E5FC", "#B2EBF2", "#B2DFDB", "#C8E6C9", "#DCEDC8", "#F0F4C3",
    "#FFF9C4", "#FFECB3", "#FFE0B2", "#FFCCBC", "#D7CCC8", "#CFD8DC",
    "#BDBDBD"
  ]

  if (count <= 0) {
    return []; // 返回空数组，因为数量无效
  }

  const result: string[] = [];

  for (let i = 0; i < count; i++) {
    // const randomIndex = Math.floor(Math.random() * list.length);
    const randomIndex = i % list.length;
    result.push(list[randomIndex]);
  }

  return result;
}