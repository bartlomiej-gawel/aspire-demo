<script setup lang="ts">
import { ref, watch } from 'vue'
import type { OrganizationEmployeeData, CreateOrganizationData } from '../types'
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
} from '@/components/ui/sheet'
import {
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
} from '@/components/ui/tabs'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Plus, Trash2 } from 'lucide-vue-next'

interface Props {
  open: boolean
}

interface Emits {
  (e: 'update:open', value: boolean): void
  (e: 'save', data: CreateOrganizationData): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const name = ref('')
const admins = ref<OrganizationEmployeeData[]>([])

watch(() => props.open, (isOpen) => {
  if (isOpen) {
    name.value = ''
    admins.value = [createEmptyAdmin()]
  }
})

function createEmptyAdmin(): OrganizationEmployeeData {
  return {
    firstName: '',
    lastName: '',
    email: '',
    phone: null,
  }
}

function addAdmin() {
  admins.value.push(createEmptyAdmin())
}

function removeAdmin(index: number) {
  if (admins.value.length > 1) {
    admins.value.splice(index, 1)
  }
}

function handleSubmit() {
  const createData: CreateOrganizationData = {
    organizationName: name.value,
    organizationAdmins: admins.value.map(admin => ({
      ...admin,
      phone: admin.phone || null,
    })),
  }
  emit('save', createData)
  emit('update:open', false)
}
</script>

<template>
  <Sheet :open="open" @update:open="emit('update:open', $event)">
    <SheetContent class="sm:max-w-[700px] overflow-hidden flex flex-col p-0">
      <div class="px-6 pt-6">
        <SheetHeader>
          <SheetTitle class="text-xl">
            Create Organization
          </SheetTitle>
          <SheetDescription>
            Add a new organization with at least one administrator.
          </SheetDescription>
        </SheetHeader>
      </div>

      <form @submit.prevent="handleSubmit" class="flex-1 flex flex-col overflow-hidden">
        <Tabs default-value="basic" class="flex-1 flex flex-col overflow-hidden">
          <div class="px-6 pt-4">
            <TabsList class="grid w-full grid-cols-2">
              <TabsTrigger value="basic">
                Basic Info
              </TabsTrigger>
              <TabsTrigger value="administrators">
                Administrators
              </TabsTrigger>
            </TabsList>
          </div>

          <div class="flex-1 overflow-y-auto px-6 pb-6">
            <TabsContent value="basic" class="space-y-5 mt-5">
              <div class="grid gap-1.5">
                <Label for="name">Organization Name</Label>
                <Input
                  id="name"
                  v-model="name"
                  placeholder="Enter organization name"
                  required
                />
              </div>
            </TabsContent>

            <TabsContent value="administrators" class="space-y-5 mt-5">
              <div class="flex items-center justify-between">
                <p class="text-sm text-muted-foreground">
                  Add at least one administrator for this organization.
                </p>
                <Button
                  type="button"
                  size="sm"
                  variant="outline"
                  @click="addAdmin"
                >
                  <Plus class="mr-2 h-4 w-4" />
                  Add Admin
                </Button>
              </div>

              <div
                v-for="(admin, index) in admins"
                :key="index"
                class="space-y-3 rounded-lg border p-4 relative"
              >
                <Button
                  v-if="admins.length > 1"
                  type="button"
                  size="sm"
                  variant="ghost"
                  @click="removeAdmin(index)"
                  class="absolute top-2 right-2 h-8 w-8 p-0"
                >
                  <Trash2 class="h-4 w-4" />
                </Button>

                <div class="grid grid-cols-2 gap-3 pt-2">
                  <div class="grid gap-1.5">
                    <Label :for="`firstName-${index}`">First Name</Label>
                    <Input
                      :id="`firstName-${index}`"
                      v-model="admin.firstName"
                      placeholder="John"
                      required
                    />
                  </div>
                  <div class="grid gap-1.5">
                    <Label :for="`lastName-${index}`">Last Name</Label>
                    <Input
                      :id="`lastName-${index}`"
                      v-model="admin.lastName"
                      placeholder="Doe"
                      required
                    />
                  </div>
                </div>

                <div class="grid gap-1.5">
                  <Label :for="`email-${index}`">Email</Label>
                  <Input
                    :id="`email-${index}`"
                    v-model="admin.email"
                    type="email"
                    placeholder="john.doe@example.com"
                    required
                  />
                </div>

                <div class="grid gap-1.5">
                  <Label :for="`phone-${index}`">Phone (Optional)</Label>
                  <Input
                    :id="`phone-${index}`"
                    :model-value="admin.phone ?? ''"
                    type="tel"
                    placeholder="+48 123 456 789"
                    @update:model-value="admin.phone = ($event as string) || null"
                  />
                </div>
              </div>
            </TabsContent>
          </div>
        </Tabs>

        <div class="border-t px-6 py-4">
          <div class="flex flex-col-reverse sm:flex-row sm:justify-end gap-3">
            <Button
              type="button"
              variant="outline"
              @click="emit('update:open', false)"
              class="w-full sm:w-auto"
            >
              Cancel
            </Button>
            <Button type="submit" class="w-full sm:w-auto">
              Create Organization
            </Button>
          </div>
        </div>
      </form>
    </SheetContent>
  </Sheet>
</template>
