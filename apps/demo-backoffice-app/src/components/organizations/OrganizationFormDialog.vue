<script setup lang="ts">
import { ref, watch } from 'vue'
import type { Organization } from '@/types/organization'
import { OrganizationStatus } from '@/types/organization'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'

interface Props {
  open: boolean
  organization: Organization | null
}

interface Emits {
  (e: 'update:open', value: boolean): void
  (e: 'save', organization: Partial<Organization>): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const name = ref('')
const status = ref<OrganizationStatus>(OrganizationStatus.Inactive)

watch(() => props.open, (isOpen) => {
  if (isOpen) {
    if (props.organization) {
      name.value = props.organization.name
      status.value = props.organization.status
    }
    else {
      name.value = ''
      status.value = OrganizationStatus.Inactive
    }
  }
})

function handleSubmit() {
  emit('save', {
    ...(props.organization && { id: props.organization.id }),
    name: name.value,
    status: status.value,
  })
  emit('update:open', false)
}
</script>

<template>
  <Dialog :open="open" @update:open="emit('update:open', $event)">
    <DialogContent class="sm:max-w-[425px]">
      <form @submit.prevent="handleSubmit">
        <DialogHeader>
          <DialogTitle>
            {{ organization ? 'Edit Organization' : 'Create Organization' }}
          </DialogTitle>
          <DialogDescription>
            {{
              organization
                ? 'Make changes to the organization details.'
                : 'Add a new organization to the system.'
            }}
          </DialogDescription>
        </DialogHeader>
        <div class="grid gap-4 py-4">
          <div class="grid gap-2">
            <Label for="name">Organization Name</Label>
            <Input
              id="name"
              v-model="name"
              placeholder="Enter organization name"
              required
            />
          </div>
          <div class="grid gap-2">
            <Label for="status">Status</Label>
            <Select v-model="status">
              <SelectTrigger id="status">
                <SelectValue placeholder="Select status" />
              </SelectTrigger>
              <SelectContent>
                <SelectGroup>
                  <SelectItem :value="OrganizationStatus.Inactive">
                    Inactive
                  </SelectItem>
                  <SelectItem :value="OrganizationStatus.Active">
                    Active
                  </SelectItem>
                  <SelectItem :value="OrganizationStatus.Archived">
                    Archived
                  </SelectItem>
                </SelectGroup>
              </SelectContent>
            </Select>
          </div>
        </div>
        <DialogFooter>
          <Button
            type="button"
            variant="outline"
            @click="emit('update:open', false)"
          >
            Cancel
          </Button>
          <Button type="submit">
            {{ organization ? 'Save Changes' : 'Create Organization' }}
          </Button>
        </DialogFooter>
      </form>
    </DialogContent>
  </Dialog>
</template>
