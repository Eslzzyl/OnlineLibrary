<template>
  <v-container>
    <v-banner sticky style="font-size: 2rem;">个人信息</v-banner>
    <v-container>
      <v-fade-transition>
        <v-alert v-if="isGetInfoErrorHappened" rounded="xl" variant="elevated" elevation="5" class="card">
          加载似乎出了一点问题。尝试刷新页面？<br>{{ requestError }}
        </v-alert>
      </v-fade-transition>
      <v-fade-transition>
        <v-card variant="elevated" elevation="5" class="card" rounded="xl">
          <v-card-title>学籍信息</v-card-title>
          <v-form>
            <v-container>
              <v-row>
                <v-col cols="3">
                  <v-text-field variant="outlined" readonly v-model="studentName" label="姓名" hint="不可修改"
                    persistent-hint></v-text-field>
                </v-col>
                <v-col cols="2">
                  <v-text-field variant="outlined" readonly v-model="studentGender" label="性别" hint="不可修改"
                    persistent-hint></v-text-field>
                </v-col>
                <v-col cols="2">
                  <v-text-field variant="outlined" readonly v-model="studentGrade" label="年级" hint="不可修改"
                    persistent-hint></v-text-field>
                </v-col>
              </v-row>
              <v-row>
                <v-col cols="3">
                  <v-text-field variant="outlined" readonly v-model="studentDept" label="院系" hint="不可修改"
                    persistent-hint></v-text-field>
                </v-col>
                <v-col cols="3">
                  <v-text-field variant="outlined" readonly v-model="studentMajor" label="专业" hint="不可修改"
                    persistent-hint></v-text-field>
                </v-col>
                <v-col cols="2">
                  <v-text-field variant="outlined" readonly v-model="studentClass" label="班级" hint="不可修改"
                    persistent-hint></v-text-field>
                </v-col>
                <v-col cols="2">
                  <v-select variant="outlined" clearable v-model="studentGraduated" label="是否已毕业"
                    :items="studentGraduatedList"></v-select>
                </v-col>
              </v-row>
            </v-container>
          </v-form>
        </v-card>
      </v-fade-transition>

      <v-fade-transition>
        <v-card variant="elevated" elevation="5" class="card" rounded="xl">
          <v-card-title>去向信息</v-card-title>
          <v-form>
            <v-container>
              <v-row>
                <v-col cols="4">
                  <v-select variant="outlined" clearable v-model="goneType" label="去向类型" :items="goneTypeList"></v-select>
                </v-col>
                <v-col>
                  <v-text-field variant="outlined" clearable v-model="goneInstitute" label="去向单位"
                    hint="输入工作单位时，建议带上岗位；输入学校时，建议带上专业方向" persistent-hint></v-text-field>
                </v-col>
              </v-row>
              <v-row>
                <v-col>
                  <v-textarea variant="outlined" v-model="comments" label="备注" hint="可以在这里输入其他想说的内容，如上岸经验、额外的自我介绍等"
                    persistent-hint></v-textarea>
                </v-col>
              </v-row>
            </v-container>
          </v-form>
        </v-card>
      </v-fade-transition>

      <v-fade-transition>
        <v-card variant="elevated" elevation="5" class="card" rounded="xl">
          <v-card-title>联系方式信息</v-card-title>
          <v-form>
            <v-container>
              <v-row>
                <v-col>
                  <v-text-field variant="outlined" clearable v-model="contactMail" label="邮箱"></v-text-field>
                </v-col>
                <v-col>
                  <v-text-field variant="outlined" clearable v-model="contactPhone" label="电话"></v-text-field>
                </v-col>
              </v-row>
              <v-row>
                <v-col>
                  <v-text-field variant="outlined" clearable v-model="contactQQ" label="QQ"></v-text-field>
                </v-col>
                <v-col>
                  <v-text-field variant="outlined" clearable v-model="contactWeChat" label="微信"></v-text-field>
                </v-col>
              </v-row>
              <v-row>
                <v-col>
                  <v-textarea variant="outlined" v-model="contactOthers" label="其他联系方式"></v-textarea>
                </v-col>
              </v-row>
            </v-container>
          </v-form>
        </v-card>
      </v-fade-transition>

      <v-fab-transition>
        <v-card variant="elevated" elevation="5" class="card" rounded="xl">
          <v-card-title>账号</v-card-title>
          <v-form fast-fail @submit.prevent>
            <v-container>
              <v-row>
                <v-col>
                  学号：{{ studentID }}
                </v-col>
              </v-row>
              <v-row>
                <v-divider></v-divider>
              </v-row>
              <v-row>
                <v-col cols="3">
                  头像：
                  <v-avatar :image="studentAvatar !== '' && studentAvatar !== 'null' ? studentAvatar : '/avatar/chino.jpg'" size="64"></v-avatar>
                </v-col>
              </v-row>
              <v-row>
                <v-col cols="4">
                  <v-btn variant="tonal" :loading="isSelectingAvatar" @click="handleAvatarButton">上传新头像</v-btn>
                  <!-- Create a File Input that will be hidden but triggered with JavaScript -->
                  <input v-show="false" ref="avatarSelector" type="file" @change="updateAvatar">
                </v-col>
              </v-row>
              <v-row>
                <v-divider></v-divider>
              </v-row>
              <v-row>
                <v-col>
                  修改密码：
                </v-col>
              </v-row>
              <v-row>
                <v-col>
                  <v-text-field variant="outlined" v-model="newPassword" label="新密码"></v-text-field>
                </v-col>
                <v-col>
                  <v-text-field variant="outlined" v-model="newPasswordConfirmed" label="再次输入以确认密码"
                    :rules="passwordConfirmRules"></v-text-field>
                </v-col>
              </v-row>
            </v-container>
          </v-form>
        </v-card>
      </v-fab-transition>
      <v-btn @click="updateInfo">提交更改</v-btn>
      <v-fade-transition>
        <v-alert v-if="isUpdateInfoErrorHappened" rounded="xl" variant="elevated" elevation="5" class="card">
          上传数据时似乎出了一点问题。尝试重新上传？<br>{{ requestError }}
        </v-alert>
      </v-fade-transition>
    </v-container>
    <v-snackbar v-model="snackbar" timeout="5000" rounded="pill" color="indigo-lighten-4">
      {{ prompt }}
      <v-btn variant="text" @click="snackbar = false">关闭</v-btn>
    </v-snackbar>
  </v-container>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import axiosInstance from '@/plugins/util/axiosInstance';
import axios from 'axios';
import { calculate_sha256 } from '@/plugins/util/encrypt';

const studentName = ref('')
const studentGender = ref('')
const studentClass = ref('')
const studentGrade = ref('')
const studentGraduated = ref('')
const studentGraduatedList = ['是', '否']
const studentDept = ref('')
const studentMajor = ref('')

const goneType = ref('')
const goneTypeList = ref(['升学', '就业', '出国'])
const goneInstitute = ref('')

const contactQQ = ref('')
const contactWeChat = ref('')
const contactMail = ref('')
const contactPhone = ref('')
const contactOthers = ref('')

const comments = ref('')

const studentID = ref('999999')
const newPassword = ref('')
const newPasswordConfirmed = ref('')

const studentAvatar = ref('')
const isSelectingAvatar = ref(false)
const isAvatarUpdated = ref(false)

const isGetInfoErrorHappened = ref(false)
const isUpdateInfoErrorHappened = ref(false)
const requestError = ref()

const snackbar = ref(false)
const prompt = ref('')

const emit = defineEmits<{
  (event: 'change-avatar', avatar: string): void
}>()

// https://cn.vuejs.org/api/built-in-special-attributes.html#ref
const avatarSelector = ref<HTMLElement>()

const passwordConfirmRules = [
  () => {
    if (newPassword.value === newPasswordConfirmed.value) {
      return true
    } else {
      return '两次输入的密码不一致'
    }
  },
]

onMounted(() => {
  let id = window.localStorage.getItem("id")
  if (id) {
    studentID.value = id
  }
  axiosInstance
    .post('/user/getinfo', {
      id: studentID.value
    })
    .then((response) => {
      console.log(response)
      if (response.data.code === 1) {
        console.log('请求成功')
        isGetInfoErrorHappened.value = false
        studentName.value = response.data.data.name
        studentGender.value = response.data.data.gender
        studentGrade.value = response.data.data.grade
        studentDept.value = response.data.data.dept
        studentMajor.value = response.data.data.major
        studentClass.value = response.data.data.class
        studentGraduated.value = response.data.data.graduated
        goneType.value = response.data.data.goneType
        goneTypeList.value = response.data.data.goneTypeList
        goneInstitute.value = response.data.data.gone
        comments.value = response.data.data.comments
        contactMail.value = response.data.data.mail
        contactPhone.value = response.data.data.phone
        contactQQ.value = response.data.data.qq
        contactWeChat.value = response.data.data.wechat
        contactOthers.value = response.data.data.others
      } else {
        isGetInfoErrorHappened.value = true
        console.log('请求失败！')
      }
    }).catch((error) => {
      console.log(error)
      requestError.value = error
      isGetInfoErrorHappened.value = true
    })
})

async function updateInfo() {
  let hashed_password;
  if (newPassword.value != '') {
    hashed_password = calculate_sha256(newPassword.value);
  } else {
    hashed_password = window.localStorage.getItem("hashed_password");
  }
  axiosInstance.post('/user/updateinfo', {
    id: studentID.value,
    password: hashed_password,
    avatar: studentAvatar.value,
    graduated: studentGraduated.value,
    goneType: goneType.value,
    gone: goneInstitute.value,
    comments: comments.value,
    mail: contactMail.value,
    phone: contactPhone.value,
    wechat: contactWeChat.value,
    qq: contactQQ.value,
    others: contactOthers.value
  }).then((response) => {
    console.log(response)
    if (response.data.code === 1) {
      console.log('请求成功')
      isUpdateInfoErrorHappened.value = false
      prompt.value = '更新成功'
      snackbar.value = true
    } else {
      isUpdateInfoErrorHappened.value = true
      console.error('请求失败！', response.data.message)
      requestError.value = response.data.message
    }
  }).catch((error) => {
    console.error(error)
    requestError.value = error.message
    isUpdateInfoErrorHappened.value = true
  })
}

// 这个函数仅仅用于唤起文件选择对话框，不承载任何业务逻辑
function handleAvatarButton() {
  isSelectingAvatar.value = true

  // After obtaining the focus when closing the FilePicker, return the button state to normal
  window.addEventListener('focus', () => {
    isSelectingAvatar.value = false
  }, { once: true });

  // Trigger click on the FileInput
  (avatarSelector.value as HTMLElement).click();
}

function updateAvatar(e: Event) {
  isSelectingAvatar.value = true    // 上传时也转圈
  const target = e.target as HTMLInputElement
  if (target !== null && target.files !== null) {
    isAvatarUpdated.value = true
    const selectedAvatar = target.files[0]
    const formData = new FormData();
    formData.append('file', selectedAvatar);

    axios.post("https://img.eslzzyl.eu.org/upload", formData)
      .then((response) => {
        studentAvatar.value = response.data
        window.localStorage.setItem("avatar", studentAvatar.value)
        prompt.value = "头像上传成功，你仍然需要点击下面的提交按钮来提交更改"
        snackbar.value = true
        console.log("新头像URL：", studentAvatar.value)
        isSelectingAvatar.value = false
        emit('change-avatar', studentAvatar.value)
      }).catch((error) => {
        console.error('上传头像时遇到问题：', error)
        prompt.value = "上传头像时遇到问题：" + error.message
        snackbar.value = true
        isSelectingAvatar.value = false
        return
      })
  }
}

</script>

<style scoped>.card {
  margin: 0 auto;
  margin-bottom: 15px;
  margin-top: 15px;
}</style>