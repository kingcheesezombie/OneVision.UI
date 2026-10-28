const overlays = new Map();

const selector = [
  'a[href]',
  'button:not([disabled])',
  'input:not([disabled]):not([type="hidden"])',
  'select:not([disabled])',
  'textarea:not([disabled])',
  '[tabindex]:not([tabindex="-1"])',
  '[contenteditable="true"]'
].join(',');

function focusables(surface) {
  return Array.from(surface.querySelectorAll(selector)).filter(element => {
    const style = window.getComputedStyle(element);
    return style.visibility !== 'hidden' && style.display !== 'none' && !element.hasAttribute('aria-hidden');
  });
}

export function activate(id, surface, dotnet, lockScroll) {
  if (!surface) return;
  const trigger = document.activeElement instanceof HTMLElement ? document.activeElement : null;
  const previousOverflow = document.body.style.overflow;
  const previousPaddingRight = document.body.style.paddingRight;

  if (lockScroll) {
    const scrollbar = window.innerWidth - document.documentElement.clientWidth;
    document.body.style.overflow = 'hidden';
    if (scrollbar > 0) document.body.style.paddingRight = `${scrollbar}px`;
  }

  const onKeyDown = event => {
    if (event.key === 'Escape') {
      event.preventDefault();
      event.stopPropagation();
      dotnet.invokeMethodAsync('HandleEscapeFromJs');
      return;
    }
    if (event.key !== 'Tab') return;

    const items = focusables(surface);
    if (items.length === 0) {
      event.preventDefault();
      surface.focus();
      return;
    }

    const first = items[0];
    const last = items[items.length - 1];
    const active = document.activeElement;
    if (event.shiftKey && (active === first || !surface.contains(active))) {
      event.preventDefault();
      last.focus();
    } else if (!event.shiftKey && (active === last || !surface.contains(active))) {
      event.preventDefault();
      first.focus();
    }
  };

  surface.addEventListener('keydown', onKeyDown, true);
  overlays.set(id, { surface, trigger, dotnet, onKeyDown, lockScroll, previousOverflow, previousPaddingRight });

  const items = focusables(surface);
  const preferred = items.find(x => !x.hasAttribute('data-ov-overlay-close')) ?? items.find(x => x.hasAttribute('data-ov-overlay-close'));
  (preferred ?? surface).focus();
}

export function deactivate(id, restoreFocus = true) {
  const state = overlays.get(id);
  if (!state) return;

  state.surface?.removeEventListener('keydown', state.onKeyDown, true);
  if (state.lockScroll) {
    document.body.style.overflow = state.previousOverflow;
    document.body.style.paddingRight = state.previousPaddingRight;
  }

  if (restoreFocus && state.trigger && document.contains(state.trigger)) {
    state.trigger.focus();
  }

  overlays.delete(id);
}
